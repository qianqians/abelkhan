import { Encoder } from "./Encoder";
import { Decoder } from "./Decoder";

export * from "./Encoder";
export * from "./Decoder";

/**
 * Encode a value to the MsgPack binary format. This is just a
 * convenience function: the encoder used can be access via
 * encode.encoder.
 * 
 * @param value   The data to encode.
 * @param reserve If provided and greater than the size of the
 *                current encoding buffer, a new buffer of this
 *                size will be reserved.
 */
export function encode(obj: any, initBuffSize?: number): Uint8Array
{
    return encode.encoder.encode(obj, initBuffSize);
}
encode.encoder = new Encoder();
Object.freeze(encode);

/**
 * Decode the first MsgPack value encountered. This is just a
 * convenience function: the decoder used can be accessed via
 * decode.decoder.
 * 
 * @param data The buffer to decode from.
 */
export function decode<T = any>(data: ArrayBuffer | Uint8Array): T
{
    return decode.decoder.decode<T>(data);
}
decode.decoder = new Decoder();
Object.freeze(decode);