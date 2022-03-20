"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Encoder = void 0;
const text_encoding_1 = require("./text-encoding");
/**
 * Standard timestamp extension (-1 identifier) from MessagePack spec.
 *
 * @see https://github.com/msgpack/msgpack/blob/master/spec.md#timestamp-extension-type
 */
function encodeTimestamp(d) {
    const U32_CAP = 2 ** 32;
    const ms = d.getTime();
    // CAUTION: there is a distinction between floor and other methods of integer
    // division (casting away decimal) when the milliseconds are negative, i.e.,
    // the date is before the UNIX epoch. Only the 96-bit MessagePack format
    // supports negative timestamps and even within that format, only seconds are
    // signed. Say we want to represent -34.2 seconds; the intuitive way to
    // break it up would be -34 seconds and -200 million nanoseconds. However,
    // because nanoseconds are still not signed in the 96-bit format, we must floor
    // -34.2 to -35 seconds and then use 800 million nanoseconds to raise the end
    // value back up!
    const s = Math.floor(ms / 1000);
    // Proof that 'ns' is guaranteed to be positive:
    //      1. s <= ms / 1000                   # definition of floor function
    //      2. s * 1000 <= ms                   # algebra (multiply both sides by 1000)
    //      3. 0 <= ms - (s * 1000)             # algebra (subtract s * 1000 from both sides)
    //      4. 0 <= (ms - (s * 1000)) * 1e6     # algebra (multiply both sides by 1e6)
    const ns = (ms - (s * 1000)) * 1e6;
    // If timestamp is negative we must resort the 96-bit format
    if (ms >= 0) {
        if (ns === 0 && s < U32_CAP) {
            // Only seconds and they fit in uint32 -> use 32-bit representation
            const buff = new ArrayBuffer(4);
            const view = new DataView(buff);
            // 32 bits for seconds
            view.setUint32(0, s);
            return new Uint8Array(buff);
        }
        if (s < (2 ** 34)) {
            // Seconds fit in uint34 -> use 64-bit representation
            const buff = new ArrayBuffer(8);
            const view = new DataView(buff);
            // 30 bits for nanoseconds and upper 2 bits of seconds
            view.setUint32(0, (ns << 2) + Math.floor(s / U32_CAP));
            // Lower 32 bits of seconds
            view.setUint32(4, s % U32_CAP);
            return new Uint8Array(buff);
        }
    }
    // 96-bit representation (worst case if numbers are big or negative)
    const buff = new ArrayBuffer(12);
    const view = new DataView(buff);
    // TODO: not sure if this actually encodes int64 seconds correctly
    view.setUint32(0, ns);
    view.setInt32(4, Math.trunc(s / U32_CAP));
    view.setUint32(8, s % U32_CAP);
    return new Uint8Array(buff);
}
/**
 * Class for encoding arrays, objects, and primitives to msgpack
 * format. You can create indepently configured instances, but you will
 * most likely want to configure the encode.encoder instance and simply
 * use encode().
 */
class Encoder {
    static textEncoder = new text_encoding_1.TextEncoder();
    /**
     * Registered extension decoders.
     */
    extensions = new Map();
    /**
     * Buffer is a Uint8Array "view" on top of the underlying data buffer. It compliments
     * this.view, because Uint8Array and DataView support different operations.
     */
    buffer;
    /**
     * View is a DataView "view" on top of the underlying data buffer. It compliments
     * this.buffer, because Uint8Array and DataView support different operations.
     */
    view;
    /**
     * Offset is the current location in the buffer we are encoding to.
     */
    offset = 0;
    /**
     * Register an extension encoder. Negative extension types are reserved by the spec, but
     * it is legal for you, the library user, to register encoders for such extensions in case
     * this library has not been updated to provide one or it does not fit your use case.
     */
    registerExt(constructor, type, encoderFn) {
        this.extensions.set(constructor, { type: type, fn: encoderFn });
    }
    /**
     * Release the current encoding buffer and allocate a new one.
     * @param newSize Size, in bytes, to allocate for the new buffer.
     */
    resize(newSize) {
        this.buffer = new Uint8Array(newSize);
        this.view = new DataView(this.buffer.buffer);
    }
    /**
     * Encode a value to the MsgPack binary format.
     *
     * @param value   The data to encode.
     * @param reserve If provided and greater than the size of the
     *                current encoding buffer, a new buffer of this
     *                size will be reserved.
     */
    encode(value, reserve) {
        if (typeof reserve === "number" && reserve > this.buffer.length)
            this.resize(reserve);
        this.offset = 0;
        this.recursiveEncode(value);
        return this.buffer.slice(0, this.offset);
    }
    recursiveEncode = (data) => {
        switch (typeof data) {
            case "undefined":
                this.writeNil();
                break;
            case "boolean":
                this.writeBoolean(data);
                break;
            case "number":
                this.writeNumber(data);
                break;
            case "bigint":
                this.writeBigInt(data);
                break;
            case "string":
                this.writeString(data);
                break;
            case "object":
                if (data === null) {
                    this.writeNil();
                }
                else if (this.extensions.has(data.constructor)) {
                    const { type, fn } = this.extensions.get(data.constructor);
                    this.writeExt(type, fn(data));
                }
                else if (data instanceof Uint8Array || data instanceof ArrayBuffer) {
                    this.writeBinary(data);
                }
                else if (Array.isArray(data)) {
                    this.writeArrayPrefix(data.length);
                    data.forEach(this.recursiveEncode);
                }
                else if (data instanceof Map) {
                    const validKeys = Array.from(data.keys()).filter(key => {
                        const keyType = typeof key;
                        const valType = typeof data.get(key);
                        return keyType !== "function" && valType !== "function" && valType !== "undefined";
                    });
                    this.writeMapPrefix(validKeys.length);
                    validKeys.forEach(key => {
                        this.recursiveEncode(key);
                        this.recursiveEncode(data.get(key));
                    });
                }
                else {
                    const validKeys = Object.keys(data).filter(key => {
                        const valType = typeof data[key];
                        return valType !== "undefined" && valType !== "function";
                    });
                    this.writeMapPrefix(validKeys.length);
                    validKeys.forEach(key => {
                        this.recursiveEncode(key);
                        this.recursiveEncode(data[key]);
                    });
                }
                break;
        }
    };
    ensureSufficientSpace(bytesToEncode) {
        const requiredSize = this.offset + bytesToEncode;
        if (requiredSize > this.buffer.length) {
            const newLength = Math.max(this.buffer.length * 2, requiredSize);
            const newBuffer = new Uint8Array(newLength);
            newBuffer.set(this.buffer);
            this.buffer = newBuffer;
            this.view = new DataView(this.buffer.buffer);
        }
    }
    writeNil() {
        this.ensureSufficientSpace(1);
        this.view.setUint8(this.offset++, 0xc0);
    }
    writeBoolean(value) {
        this.ensureSufficientSpace(1);
        this.view.setUint8(this.offset++, value ? 0xc3 : 0xc2);
    }
    writeNumber(value) {
        if (Number.isSafeInteger(value)) {
            this.unsafeWriteInt(value);
        }
        else {
            this.ensureSufficientSpace(9);
            this.view.setUint8(this.offset++, 0xcb);
            this.view.setFloat64(this.offset, value);
            this.offset += 8;
        }
    }
    /**
     * Encode a numeric value which has been guaranteed to be an integer
     * in the range [Number.MIN_SAFE_INTEGER, Number.MAX_SAFE_INTEGER].
     */
    unsafeWriteInt(value) {
        if (value >= 0) {
            if (value < (1 << 7)) {
                this.ensureSufficientSpace(1);
                this.view.setUint8(this.offset++, value);
            }
            else if (value < (1 << 8)) {
                this.ensureSufficientSpace(2);
                this.view.setUint8(this.offset++, 0xcc);
                this.view.setUint8(this.offset++, value);
            }
            else if (value < (1 << 16)) {
                this.ensureSufficientSpace(3);
                this.view.setUint8(this.offset++, 0xcd);
                this.view.setUint16(this.offset, value);
                this.offset += 2;
            }
            else if (value < Math.pow(2, 32)) {
                this.ensureSufficientSpace(5);
                this.view.setUint8(this.offset++, 0xce);
                this.view.setUint32(this.offset, value);
                this.offset += 4;
            }
            else // uint64; cannot use bitwise operators (casts to int32)
             {
                this.ensureSufficientSpace(9);
                this.view.setUint8(this.offset++, 0xcf);
                this.view.setUint32(this.offset, value / (2 ** 32));
                this.offset += 4;
                this.view.setUint32(this.offset, value);
                this.offset += 4;
            }
        }
        else {
            if (value >= -32) {
                this.ensureSufficientSpace(1);
                this.view.setInt8(this.offset++, value);
            }
            else if (value >= -(1 << 8)) {
                this.ensureSufficientSpace(2);
                this.view.setUint8(this.offset++, 0xd0);
                this.view.setInt8(this.offset++, value);
            }
            else if (value >= -(1 << 16)) {
                this.ensureSufficientSpace(3);
                this.view.setUint8(this.offset++, 0xd1);
                this.view.setInt16(this.offset, value);
                this.offset += 2;
            }
            else if (value >= -Math.pow(2, 32)) {
                this.ensureSufficientSpace(5);
                this.view.setUint8(this.offset++, 0xd2);
                this.view.setInt32(this.offset, value);
                this.offset += 4;
            }
            else // TODO: figure out how to encode int64
             {
                this.ensureSufficientSpace(9);
                this.view.setUint8(this.offset++, 0xcb);
                this.view.setFloat64(this.offset, value);
                this.offset += 8;
            }
        }
    }
    writeBigInt(value) {
        if (value >= BigInt(Number.MIN_SAFE_INTEGER) &&
            value <= BigInt(Number.MAX_SAFE_INTEGER)) {
            this.unsafeWriteInt(Number(value));
        }
        else if (value < 0) {
            if (value <= -(BigInt(2) ** BigInt(63)))
                throw new Error("msgpack: cannot encode BigInt less than -2^63");
            this.ensureSufficientSpace(9);
            this.view.setUint8(this.offset++, 0xd3);
            this.view.setBigInt64(this.offset, value);
            this.offset += 8;
        }
        else if (value < BigInt(2) ** BigInt(64)) {
            this.ensureSufficientSpace(9);
            this.view.setUint8(this.offset++, 0xcf);
            this.view.setBigUint64(this.offset, value);
            this.offset += 8;
        }
        else
            throw new Error("msgpack: cannot encode BigInt greater than 2^64 - 1");
    }
    writeString(value) {
        const utf8 = Encoder.textEncoder.encode(value);
        if (utf8.length < 32) {
            this.ensureSufficientSpace(1 + utf8.byteLength);
            this.view.setUint8(this.offset++, 0xa0 | utf8.byteLength);
            this.buffer.set(utf8, this.offset);
            this.offset += utf8.byteLength;
        }
        else {
            try {
                this.writeBytes(utf8, 0xd9, 0xda, 0xdb);
            }
            catch {
                // String specific error
                throw new RangeError("msgpack: string too long to encode (more than 2^32 - 1 UTF-8 runes)");
            }
        }
    }
    writeBinary(value) {
        this.writeBytes(value instanceof ArrayBuffer ? new Uint8Array(value) : value, 0xc4, 0xc5, 0xc6);
    }
    writeBytes(data, oneByteLenSeqIdentifier, twoByteLenSeqIdentifier, fourByteLenSeqIdentifier) {
        if (data.byteLength < (1 << 8)) {
            this.ensureSufficientSpace(2 + data.byteLength);
            this.view.setUint8(this.offset++, oneByteLenSeqIdentifier);
            this.view.setUint8(this.offset++, data.byteLength);
        }
        else if (data.byteLength < (1 << 16)) {
            this.ensureSufficientSpace(3 + data.byteLength);
            this.view.setUint8(this.offset++, twoByteLenSeqIdentifier);
            this.view.setUint16(this.offset, data.byteLength);
            this.offset += 2;
        }
        else if (length < Math.pow(2, 32)) {
            this.ensureSufficientSpace(5 + data.byteLength);
            this.view.setUint8(this.offset++, fourByteLenSeqIdentifier);
            this.view.setUint32(this.offset, data.byteLength);
            this.offset += 4;
        }
        else
            throw new RangeError("msgpack: buffer too long to encode (more than 2^32 - 1 bytes)");
        this.buffer.set(data, this.offset);
        this.offset += data.byteLength;
    }
    writeArrayPrefix(length) {
        if (length < 16) {
            this.ensureSufficientSpace(1);
            this.view.setUint8(this.offset++, 0x90 | length);
        }
        else if (length < (1 << 16)) {
            this.ensureSufficientSpace(3);
            this.view.setUint8(this.offset++, 0xdc);
            this.view.setUint16(this.offset, length);
            this.offset += 2;
        }
        else // ECMA dictates that array length will never exceed a uint32
         {
            this.ensureSufficientSpace(5);
            this.view.setUint8(this.offset++, 0xdd);
            this.view.setUint32(this.offset, length);
            this.offset += 4;
        }
    }
    writeMapPrefix(keyCount) {
        if (keyCount < 16) {
            this.ensureSufficientSpace(1);
            this.view.setUint8(this.offset++, 0x80 | keyCount);
        }
        else if (keyCount < (1 << 16)) {
            this.ensureSufficientSpace(3);
            this.view.setUint8(this.offset++, 0xde);
            this.view.setUint16(this.offset, keyCount);
            this.offset += 2;
        }
        else if (keyCount < Math.pow(2, 32)) {
            this.ensureSufficientSpace(5);
            this.view.setUint8(this.offset++, 0xdf);
            this.view.setUint32(this.offset, keyCount);
            this.offset += 4;
        }
        else
            throw new RangeError("msgpack: map too large to encode (more than 2^32 - 1 defined values)");
    }
    writeExt(type, data) {
        switch (data.length) {
            case 1:
                this.ensureSufficientSpace(3);
                this.buffer[this.offset++] = 0xd4;
                this.buffer[this.offset++] = type;
                this.buffer[this.offset++] = data[0];
                break;
            case 2:
                this.ensureSufficientSpace(4);
                this.buffer[this.offset++] = 0xd5;
                this.buffer[this.offset++] = type;
                this.buffer.set(data, this.offset);
                this.offset += 2;
                break;
            case 4:
                this.ensureSufficientSpace(6);
                this.buffer[this.offset++] = 0xd6;
                this.buffer[this.offset++] = type;
                this.buffer.set(data, this.offset);
                this.offset += 4;
                break;
            case 8:
                this.ensureSufficientSpace(10);
                this.buffer[this.offset++] = 0xd7;
                this.buffer[this.offset++] = type;
                this.buffer.set(data, this.offset);
                this.offset += 8;
                break;
            case 16:
                this.ensureSufficientSpace(6);
                this.buffer[this.offset++] = 0xd8;
                this.buffer[this.offset++] = type;
                this.buffer.set(data, this.offset);
                this.offset += 16;
                break;
            default:
                if (data.length < (1 << 8)) {
                    this.ensureSufficientSpace(3 + data.length);
                    this.buffer[this.offset++] = 0xc7;
                    this.buffer[this.offset++] = data.length;
                    this.buffer[this.offset++] = type;
                    this.buffer.set(data, this.offset);
                    this.offset += data.length;
                }
                else if (data.length < (1 << 16)) {
                    this.ensureSufficientSpace(4 + data.length);
                    this.buffer[this.offset++] = 0xc8;
                    this.view.setUint16(this.offset, data.length);
                    this.offset += 2;
                    this.buffer[this.offset++] = type;
                    this.buffer.set(data, this.offset);
                    this.offset += data.length;
                }
                else if (data.length < Math.pow(2, 32)) {
                    this.ensureSufficientSpace(6 + data.length);
                    this.buffer[this.offset++] = 0xc9;
                    this.view.setUint32(this.offset, data.length);
                    this.offset += 4;
                    this.buffer[this.offset++] = type;
                    this.buffer.set(data, this.offset);
                    this.offset += data.length;
                }
                else
                    throw new Error(`msgpack: ext (${type}) data too large to encode (length > 2^32 - 1)`);
        }
    }
    /**
     * Construct a new Encoder.
     * @param reserve Starting size, in bytes, for the encoding buffer.
     */
    constructor(reserve = 128) {
        this.buffer = new Uint8Array(reserve);
        this.view = new DataView(this.buffer.buffer);
        this.registerExt(Date, -1, encodeTimestamp);
    }
}
exports.Encoder = Encoder;
//# sourceMappingURL=Encoder.js.map