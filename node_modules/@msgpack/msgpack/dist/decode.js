"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.decodeMulti = exports.decode = exports.defaultDecodeOptions = void 0;
const Decoder_1 = require("./Decoder");
exports.defaultDecodeOptions = {};
/**
 * It decodes a single MessagePack object in a buffer.
 *
 * This is a synchronous decoding function.
 * See other variants for asynchronous decoding: {@link decodeAsync()}, {@link decodeStream()}, or {@link decodeArrayStream()}.
 */
function decode(buffer, options = exports.defaultDecodeOptions) {
    const decoder = new Decoder_1.Decoder(options.extensionCodec, options.context, options.maxStrLength, options.maxBinLength, options.maxArrayLength, options.maxMapLength, options.maxExtLength);
    return decoder.decode(buffer);
}
exports.decode = decode;
/**
 * It decodes multiple MessagePack objects in a buffer.
 * This is corresponding to {@link decodeMultiStream()}.
 */
function decodeMulti(buffer, options = exports.defaultDecodeOptions) {
    const decoder = new Decoder_1.Decoder(options.extensionCodec, options.context, options.maxStrLength, options.maxBinLength, options.maxArrayLength, options.maxMapLength, options.maxExtLength);
    return decoder.decodeMulti(buffer);
}
exports.decodeMulti = decodeMulti;
//# sourceMappingURL=decode.js.map