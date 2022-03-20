import { TextDecoder } from "./text-encoding"

const enum MsgPack
{
    PosFixInt,
    FixMap,
    FixArray,
    FixString,
    Nil,
    NeverUsed,
    False,
    True,
    Bin8,
    Bin16,
    Bin32,
    Ext8,
    Ext16,
    Ext32,
    Float32,
    Float64,
    Uint8,
    Uint16,
    Uint32,
    Uint64,
    Int8,
    Int16,
    Int32,
    Int64,
    FixExt1,
    FixExt2,
    FixExt4,
    FixExt8,
    FixExt16,
    String8,
    String16,
    String32,
    Array16,
    Array32,
    Map16,
    Map32,
    NegFixInt
}

type ExtDecoderFn = (data: Uint8Array) => any;

const identifierToType: Uint8Array = function()
{
    const arr = new Uint8Array(256);

    for (let i = 0x00; i <= 0x7f; ++i)
        arr[i] = MsgPack.PosFixInt;
    for (let i = 0x80; i <= 0x8f; ++i)
        arr[i] = MsgPack.FixMap;
    for (let i = 0x90; i <= 0x9f; ++i)
        arr[i] = MsgPack.FixArray;
    for (let i = 0xa0; i <= 0xbf; ++i)
        arr[i] = MsgPack.FixString;

    arr[0xc0] = MsgPack.Nil;
    arr[0xc1] = MsgPack.NeverUsed;
    arr[0xc2] = MsgPack.False;
    arr[0xc3] = MsgPack.True;
    arr[0xc4] = MsgPack.Bin8;
    arr[0xc5] = MsgPack.Bin16;
    arr[0xc6] = MsgPack.Bin32;
    arr[0xc7] = MsgPack.Ext8;
    arr[0xc8] = MsgPack.Ext16;
    arr[0xc9] = MsgPack.Ext32;
    arr[0xca] = MsgPack.Float32;
    arr[0xcb] = MsgPack.Float64;
    arr[0xcc] = MsgPack.Uint8;
    arr[0xcd] = MsgPack.Uint16;
    arr[0xce] = MsgPack.Uint32;
    arr[0xcf] = MsgPack.Uint64;
    arr[0xd0] = MsgPack.Int8;
    arr[0xd1] = MsgPack.Int16;
    arr[0xd2] = MsgPack.Int32;
    arr[0xd3] = MsgPack.Int64;
    arr[0xd4] = MsgPack.FixExt1;
    arr[0xd5] = MsgPack.FixExt2;
    arr[0xd6] = MsgPack.FixExt4;
    arr[0xd7] = MsgPack.FixExt8;
    arr[0xd8] = MsgPack.FixExt16;
    arr[0xd9] = MsgPack.String8;
    arr[0xda] = MsgPack.String16;
    arr[0xdb] = MsgPack.String32;
    arr[0xdc] = MsgPack.Array16;
    arr[0xdd] = MsgPack.Array32;
    arr[0xde] = MsgPack.Map16;
    arr[0xdf] = MsgPack.Map32;

    for (let i = 0xe0; i <= 0xff; ++i)
        arr[i] = MsgPack.NegFixInt;

    return arr;
}()

/**
 * Standard timestamp extension (-1 identifier) from MessagePack spec.
 * 
 * @see https://github.com/msgpack/msgpack/blob/master/spec.md#timestamp-extension-type
 */
function decodeTimestamp(data: Uint8Array): Date
{
    const view = new DataView(data.buffer, data.byteOffset, data.length);

    switch (data.length)
    {
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
export class Decoder
{
    private static textDecoder = new TextDecoder("utf-8", { fatal: true });

    /**
     * Value to deserialize MsgPack's `Nil` type as. Should be set to either `null` or
     * `undefined`; default is `null`.
     */
    nilValue: null | undefined = null;

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
    mapBehavior = Decoder.MapBehavior.PreferJSON;

    /**
     * Registered extension decoders.
     */
    private readonly extensions = new Map<number, ExtDecoderFn>();

    /**
     * Buffer is a Uint8Array "view" on top of the buffer being decoded. It compliments
     * this.view, because Uint8Array and DataView support different operations.
     */
    private buffer!: Uint8Array;

    /**
     * View is a DataView "view" on top of the buffer being decoded. It compliments this.buffer,
     * because Uint8Array and DataView support different operations.
     */
    private view!: DataView;

    /**
     * Offset is the current location in the buffer we are decoding.
     */
    private offset!: number;

    /**
     * Create a new Decoder with the same configuration and extension decoders.
     */
    clone(): Decoder
    {
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
    registerExt(type: number, decoderFn: ExtDecoderFn)
    {
        this.extensions.set(type, decoderFn);
    }

    /**
     * Decode the first MsgPack value encountered.
     * 
     * @param data The buffer to decode from.
     */
    decode<T = any>(data: ArrayBuffer | Uint8Array): T
    {
        this.buffer = data instanceof Uint8Array ? data : new Uint8Array(data);
        this.view = new DataView(this.buffer.buffer, this.buffer.byteOffset);
        this.offset = 0;

        return this.nextObject();
    }

    private nextObject(): any
    {
        const seqByte = this.takeUint8();
        const seqType = identifierToType[seqByte] as MsgPack;

        switch (seqType)
        {
            case MsgPack.PosFixInt: return seqByte;
            case MsgPack.FixMap:    return this.takeMap(seqByte & 0xf);
            case MsgPack.FixArray:  return this.takeArray(seqByte & 0xf);
            case MsgPack.FixString: return this.takeString(seqByte & 0x1f);
            case MsgPack.Nil:       return this.nilValue;
            case MsgPack.NeverUsed: return undefined;
            case MsgPack.False:     return false;
            case MsgPack.True:      return true;
            case MsgPack.Bin8:      return this.takeBinary(this.takeUint8());
            case MsgPack.Bin16:     return this.takeBinary(this.takeUint16());
            case MsgPack.Bin32:     return this.takeBinary(this.takeUint32());
            case MsgPack.Ext8:      return this.takeExt(this.takeUint8());
            case MsgPack.Ext16:     return this.takeExt(this.takeUint16());
            case MsgPack.Ext32:     return this.takeExt(this.takeUint32());
            case MsgPack.Float32:   return this.takeFloat32();
            case MsgPack.Float64:   return this.takeFloat64();
            case MsgPack.Uint8:     return this.takeUint8();
            case MsgPack.Uint16:    return this.takeUint16();
            case MsgPack.Uint32:    return this.takeUint32();
            case MsgPack.Uint64:    return this.takeUint64();
            case MsgPack.Int8:      return this.takeInt8();
            case MsgPack.Int16:     return this.takeInt16();
            case MsgPack.Int32:     return this.takeInt32();
            case MsgPack.Int64:     return this.takeInt64();
            case MsgPack.FixExt1:   return this.takeExt(1);
            case MsgPack.FixExt2:   return this.takeExt(2);
            case MsgPack.FixExt4:   return this.takeExt(4);
            case MsgPack.FixExt8:   return this.takeExt(8);
            case MsgPack.FixExt16:  return this.takeExt(16);
            case MsgPack.String8:   return this.takeString(this.takeUint8());
            case MsgPack.String16:  return this.takeString(this.takeUint16());
            case MsgPack.String32:  return this.takeString(this.takeUint32());
            case MsgPack.Array16:   return this.takeArray(this.takeUint16());
            case MsgPack.Array32:   return this.takeArray(this.takeUint32());
            case MsgPack.Map16:     return this.takeMap(this.takeUint16());
            case MsgPack.Map32:     return this.takeMap(this.takeUint32());
            case MsgPack.NegFixInt: return seqByte - 256;
        }
    }

    private takeUint8(): number
    {
        return this.view.getUint8(this.offset++);
    }

    private takeUint16(): number
    {
        const value = this.view.getUint16(this.offset);
        this.offset += 2;
        return value;
    }

    private takeUint32(): number
    {
        const value = this.view.getUint32(this.offset);
        this.offset += 4;
        return value;
    }

    private takeUint64(): number | bigint
    {
        const hi32 = this.view.getUint32(this.offset);

        if (hi32 >= (1 << 21))
        {
            if (typeof BigInt === "function")
            {
                const value = this.view.getBigUint64(this.offset);
                this.offset += 8;
                return value;
            }
            throw new Error("msgpack: 64-bit unsigned integer exceeds max safe JS integer value");
        }

        this.offset += 4;
        const value = hi32 * (2**32) + this.view.getUint32(this.offset);
        this.offset += 4;
        return value;
    }

    private takeInt8(): number
    {
        return this.view.getInt8(this.offset++);
    }

    private takeInt16(): number
    {
        const value = this.view.getInt16(this.offset);
        this.offset += 2;
        return value;
    }

    private takeInt32(): number
    {
        const value = this.view.getInt32(this.offset);
        this.offset += 4;
        return value;
    }

    private takeInt64(): number | bigint
    {
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

    private takeFloat32(): number
    {
        const value = this.view.getFloat32(this.offset);
        this.offset += 4;
        return value;
    }

    private takeFloat64(): number
    {
        const value = this.view.getFloat64(this.offset);
        this.offset += 8;
        return value;
    }

    private takeBinary(length: number): Uint8Array
    {
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

    private takeString(length: number): string | Uint8Array
    {
        const start = this.offset;
        this.offset += length;
        const utf8 = this.buffer.subarray(start, this.offset);

        try
        {
            return Decoder.textDecoder.decode(utf8);
        }
        catch (error)
        {
            if (this.allowInvalidUTF8)
                return utf8.slice();
            else
                throw error;
        }
    }

    private takeArray(length: number): any[]
    {
        const arr = [];

        for (let i = 0; i < length; ++i)
            arr.push(this.nextObject());

        return arr;
    }

    private takeMap(keyCount: number, behavior = this.mapBehavior): object | Map<any, any>
    {
        switch (behavior)
        {
            case Decoder.MapBehavior.AlwaysJSON:
            {
                const map: {[key: string]: any} = {};

                for (let i = 0; i < keyCount; ++i)
                    map[this.nextObject()] = this.nextObject();

                return map;
            }
            case Decoder.MapBehavior.AlwaysES6Map:
            {
                const map = new Map();

                for (let i = 0; i < keyCount; ++i)
                    map.set(this.nextObject(), this.nextObject());

                return map;
            }
            case Decoder.MapBehavior.PreferJSON:
            default:
            {
                const mapStart = this.offset;
                const map: {[key: string]: any} = {};

                for (let i = 0; i < keyCount; ++i)
                {
                    const key = this.nextObject();

                    if (typeof key === "object")
                    {
                        this.offset = mapStart;
                        return this.takeMap(keyCount, Decoder.MapBehavior.AlwaysES6Map);
                    }

                    map[key] = this.nextObject();
                }

                return map;
            }
        }
    }

    private takeExt(dataLength: number): any
    {
        const type = this.takeInt8();

        if (this.extensions.has(type))
            return this.extensions.get(type)!(this.takeBinary(dataLength));
        
        if (this.allowUnknownExts)
            return <Decoder.UnknownExt>{
                type: type,
                data: this.takeBinary(dataLength)
            };
        
        throw new RangeError(`msgpack: decode: unrecognized ext type (${type})`);
    }

    constructor()
    {
        this.extensions.set(-1, decodeTimestamp);
    }
}

export namespace Decoder
{
    export const enum MapBehavior
    {
        /**
         * Maps will be decoded as native JS objects, unless a key is decoded
         * whose JS type evaluates to `object`, in which case all decoded keys
         * will be abandoned and the map will be decoded from scratch into an
         * ES6 Map which supports arbitrary key types.
         */
        PreferJSON,

        /**
         * Maps will be always be decoded as native JS objects. This means all
         * decoded keys will be coerced to strings, which is almost certainly
         * undesirable if decoding maps with objects or arrays as keys.
         */
        AlwaysJSON,

        /**
         * Maps will always be decoded as ES6 Map objects.
         */
        AlwaysES6Map
    }

    /**
     * UnknownExt describes an unrecognized extension sequence that
     * was encountered during decoding and passed through opaquely.
     */
    export interface UnknownExt
    {
        /**
         * Extension type identifier.
         */
        readonly type: number;
    
        /**
         * Extension sequence data.
         */
        readonly data: Uint8Array;
    }
}