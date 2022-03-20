"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Decoder = void 0;
const text_encoding_1 = require("./text-encoding");
const identifierToType = function () {
    const arr = new Uint8Array(256);
    for (let i = 0x00; i <= 0x7f; ++i)
        arr[i] = 0 /* PosFixInt */;
    for (let i = 0x80; i <= 0x8f; ++i)
        arr[i] = 1 /* FixMap */;
    for (let i = 0x90; i <= 0x9f; ++i)
        arr[i] = 2 /* FixArray */;
    for (let i = 0xa0; i <= 0xbf; ++i)
        arr[i] = 3 /* FixString */;
    arr[0xc0] = 4 /* Nil */;
    arr[0xc1] = 5 /* NeverUsed */;
    arr[0xc2] = 6 /* False */;
    arr[0xc3] = 7 /* True */;
    arr[0xc4] = 8 /* Bin8 */;
    arr[0xc5] = 9 /* Bin16 */;
    arr[0xc6] = 10 /* Bin32 */;
    arr[0xc7] = 11 /* Ext8 */;
    arr[0xc8] = 12 /* Ext16 */;
    arr[0xc9] = 13 /* Ext32 */;
    arr[0xca] = 14 /* Float32 */;
    arr[0xcb] = 15 /* Float64 */;
    arr[0xcc] = 16 /* Uint8 */;
    arr[0xcd] = 17 /* Uint16 */;
    arr[0xce] = 18 /* Uint32 */;
    arr[0xcf] = 19 /* Uint64 */;
    arr[0xd0] = 20 /* Int8 */;
    arr[0xd1] = 21 /* Int16 */;
    arr[0xd2] = 22 /* Int32 */;
    arr[0xd3] = 23 /* Int64 */;
    arr[0xd4] = 24 /* FixExt1 */;
    arr[0xd5] = 25 /* FixExt2 */;
    arr[0xd6] = 26 /* FixExt4 */;
    arr[0xd7] = 27 /* FixExt8 */;
    arr[0xd8] = 28 /* FixExt16 */;
    arr[0xd9] = 29 /* String8 */;
    arr[0xda] = 30 /* String16 */;
    arr[0xdb] = 31 /* String32 */;
    arr[0xdc] = 32 /* Array16 */;
    arr[0xdd] = 33 /* Array32 */;
    arr[0xde] = 34 /* Map16 */;
    arr[0xdf] = 35 /* Map32 */;
    for (let i = 0xe0; i <= 0xff; ++i)
        arr[i] = 36 /* NegFixInt */;
    return arr;
}();
/**
 * Standard timestamp extension (-1 identifier) from MessagePack spec.
 *
 * @see https://github.com/msgpack/msgpack/blob/master/spec.md#timestamp-extension-type
 */
function decodeTimestamp(data) {
    const view = new DataView(data.buffer, data.byteOffset, data.length);
    switch (data.length) {
        case 4:
            {
                return new Date(view.getUint32(0) * 1000);
            }
        case 8:
            {
                const first32Bits = view.getUint32(0);
                const nano = first32Bits >>> 2;
                const sec = ((first32Bits & 0x3) * Math.pow(2, 32)) + view.getUint32(4);
                return new Date((sec * 1000) + Math.floor(nano / 1e6));
            }
        case 12:
            {
                // TODO: not sure if this actually decodes the int64 seconds correctly
                const nano = view.getUint32(0);
                const sec = (view.getInt32(4) * Math.pow(2, 32)) + view.getUint32(8);
                const ms = (sec * 1000) + Math.floor(nano / 1e6);
                if (!Number.isSafeInteger(ms))
                    throw new RangeError("msgpack: decodeDate (ext -1): timestamp exceeds safe JS integer range");
            }
        default:
            throw new RangeError(`msgpack: decodeDate (ext -1): invalid data length (${data.length})`);
    }
}
/**
 * Class for decoding maps (objects), arrays, buffers, and primitives from MsgPack format.
 */
class Decoder {
    static textDecoder = new text_encoding_1.TextDecoder("utf-8", { fatal: true });
    /**
     * Value to deserialize MsgPack's `Nil` type as. Should be set to either `null` or
     * `undefined`; default is `null`.
     */
    nilValue = null;
    /**
     * Determines behavior when a `String` value is encountered whose data is not valid UTF-8.
     * If true, deserializing the value will simply produce a raw Uint8Array containing the
     * data. Otherwise, a TypeError will be thrown and decoding will fail (default).
     */
    allowInvalidUTF8 = false;
    /**
     * If false (default), any attempt to decode an unregistered extension type will raise an
     * error. If true, unrecognized extension types will be passed through as UnknownExtSeq
     * objects.
     */
    allowUnknownExts = false;
    /**
     * Determines how MsgPack maps are decoded.
     */
    mapBehavior = 0 /* PreferJSON */;
    /**
     * Registered extension decoders.
     */
    extensions = new Map();
    /**
     * Buffer is a Uint8Array "view" on top of the buffer being decoded. It compliments
     * this.view, because Uint8Array and DataView support different operations.
     */
    buffer;
    /**
     * View is a DataView "view" on top of the buffer being decoded. It compliments this.buffer,
     * because Uint8Array and DataView support different operations.
     */
    view;
    /**
     * Offset is the current location in the buffer we are decoding.
     */
    offset;
    /**
     * Create a new Decoder with the same configuration and extension decoders.
     */
    clone() {
        const d = new Decoder();
        d.nilValue = this.nilValue;
        d.allowInvalidUTF8 = this.allowInvalidUTF8;
        d.mapBehavior = this.mapBehavior;
        this.extensions.forEach((fn, type) => d.registerExt(type, fn));
        return d;
    }
    /**
     * Register an extension decoder. Negative extension types are reserved by the spec, but
     * it is legal for you, the library user, to register decoders for such extensions in case
     * this library has not been updated to provide one or it does not fit your use case.
     */
    registerExt(type, decoderFn) {
        this.extensions.set(type, decoderFn);
    }
    /**
     * Decode the first MsgPack value encountered.
     *
     * @param data The buffer to decode from.
     */
    decode(data) {
        this.buffer = data instanceof Uint8Array ? data : new Uint8Array(data);
        this.view = new DataView(this.buffer.buffer, this.buffer.byteOffset);
        this.offset = 0;
        return this.nextObject();
    }
    nextObject() {
        const seqByte = this.takeUint8();
        const seqType = identifierToType[seqByte];
        switch (seqType) {
            case 0 /* PosFixInt */: return seqByte;
            case 1 /* FixMap */: return this.takeMap(seqByte & 0xf);
            case 2 /* FixArray */: return this.takeArray(seqByte & 0xf);
            case 3 /* FixString */: return this.takeString(seqByte & 0x1f);
            case 4 /* Nil */: return this.nilValue;
            case 5 /* NeverUsed */: return undefined;
            case 6 /* False */: return false;
            case 7 /* True */: return true;
            case 8 /* Bin8 */: return this.takeBinary(this.takeUint8());
            case 9 /* Bin16 */: return this.takeBinary(this.takeUint16());
            case 10 /* Bin32 */: return this.takeBinary(this.takeUint32());
            case 11 /* Ext8 */: return this.takeExt(this.takeUint8());
            case 12 /* Ext16 */: return this.takeExt(this.takeUint16());
            case 13 /* Ext32 */: return this.takeExt(this.takeUint32());
            case 14 /* Float32 */: return this.takeFloat32();
            case 15 /* Float64 */: return this.takeFloat64();
            case 16 /* Uint8 */: return this.takeUint8();
            case 17 /* Uint16 */: return this.takeUint16();
            case 18 /* Uint32 */: return this.takeUint32();
            case 19 /* Uint64 */: return this.takeUint64();
            case 20 /* Int8 */: return this.takeInt8();
            case 21 /* Int16 */: return this.takeInt16();
            case 22 /* Int32 */: return this.takeInt32();
            case 23 /* Int64 */: return this.takeInt64();
            case 24 /* FixExt1 */: return this.takeExt(1);
            case 25 /* FixExt2 */: return this.takeExt(2);
            case 26 /* FixExt4 */: return this.takeExt(4);
            case 27 /* FixExt8 */: return this.takeExt(8);
            case 28 /* FixExt16 */: return this.takeExt(16);
            case 29 /* String8 */: return this.takeString(this.takeUint8());
            case 30 /* String16 */: return this.takeString(this.takeUint16());
            case 31 /* String32 */: return this.takeString(this.takeUint32());
            case 32 /* Array16 */: return this.takeArray(this.takeUint16());
            case 33 /* Array32 */: return this.takeArray(this.takeUint32());
            case 34 /* Map16 */: return this.takeMap(this.takeUint16());
            case 35 /* Map32 */: return this.takeMap(this.takeUint32());
            case 36 /* NegFixInt */: return seqByte - 256;
        }
    }
    takeUint8() {
        return this.view.getUint8(this.offset++);
    }
    takeUint16() {
        const value = this.view.getUint16(this.offset);
        this.offset += 2;
        return value;
    }
    takeUint32() {
        const value = this.view.getUint32(this.offset);
        this.offset += 4;
        return value;
    }
    takeUint64() {
        const hi32 = this.view.getUint32(this.offset);
        if (hi32 >= (1 << 21)) {
            if (typeof BigInt === "function") {
                const value = this.view.getBigUint64(this.offset);
                this.offset += 8;
                return value;
            }
            throw new Error("msgpack: 64-bit unsigned integer exceeds max safe JS integer value");
        }
        this.offset += 4;
        const value = hi32 * (2 ** 32) + this.view.getUint32(this.offset);
        this.offset += 4;
        return value;
    }
    takeInt8() {
        return this.view.getInt8(this.offset++);
    }
    takeInt16() {
        const value = this.view.getInt16(this.offset);
        this.offset += 2;
        return value;
    }
    takeInt32() {
        const value = this.view.getInt32(this.offset);
        this.offset += 4;
        return value;
    }
    takeInt64() {
        // TODO: inspect high 4 bytes and support numbers within safe range
        // for regular JS number (-2^53, 2^53)
        if (true) {
            if (typeof BigInt === "function") {
                const value = this.view.getBigInt64(this.offset);
                this.offset += 8;
                return value;
            }
        }
        throw new TypeError("msgpack: 64-bit signed integer exceeds max safe JS integer value");
    }
    takeFloat32() {
        const value = this.view.getFloat32(this.offset);
        this.offset += 4;
        return value;
    }
    takeFloat64() {
        const value = this.view.getFloat64(this.offset);
        this.offset += 8;
        return value;
    }
    takeBinary(length) {
        /**
         * TODO: Need to check if spec guarantees arguments to be evaluated
         * left-to-right so we can condense this to:
         *
         * return this.buffer.subarray(this.offset, this.offset += length);
         */
        const start = this.offset;
        this.offset += length;
        return this.buffer.slice(start, this.offset);
    }
    takeString(length) {
        const start = this.offset;
        this.offset += length;
        const utf8 = this.buffer.subarray(start, this.offset);
        try {
            return Decoder.textDecoder.decode(utf8);
        }
        catch (error) {
            if (this.allowInvalidUTF8)
                return utf8.slice();
            else
                throw error;
        }
    }
    takeArray(length) {
        const arr = [];
        for (let i = 0; i < length; ++i)
            arr.push(this.nextObject());
        return arr;
    }
    takeMap(keyCount, behavior = this.mapBehavior) {
        switch (behavior) {
            case 1 /* AlwaysJSON */:
                {
                    const map = {};
                    for (let i = 0; i < keyCount; ++i)
                        map[this.nextObject()] = this.nextObject();
                    return map;
                }
            case 2 /* AlwaysES6Map */:
                {
                    const map = new Map();
                    for (let i = 0; i < keyCount; ++i)
                        map.set(this.nextObject(), this.nextObject());
                    return map;
                }
            case 0 /* PreferJSON */:
            default:
                {
                    const mapStart = this.offset;
                    const map = {};
                    for (let i = 0; i < keyCount; ++i) {
                        const key = this.nextObject();
                        if (typeof key === "object") {
                            this.offset = mapStart;
                            return this.takeMap(keyCount, 2 /* AlwaysES6Map */);
                        }
                        map[key] = this.nextObject();
                    }
                    return map;
                }
        }
    }
    takeExt(dataLength) {
        const type = this.takeInt8();
        if (this.extensions.has(type))
            return this.extensions.get(type)(this.takeBinary(dataLength));
        if (this.allowUnknownExts)
            return {
                type: type,
                data: this.takeBinary(dataLength)
            };
        throw new RangeError(`msgpack: decode: unrecognized ext type (${type})`);
    }
    constructor() {
        this.extensions.set(-1, decodeTimestamp);
    }
}
exports.Decoder = Decoder;
//# sourceMappingURL=Decoder.js.map