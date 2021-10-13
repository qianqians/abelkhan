"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.decodeStream = exports.decodeMultiStream = exports.decodeArrayStream = exports.decodeAsync = void 0;
const Decoder_1 = require("./Decoder");
const stream_1 = require("./utils/stream");
const decode_1 = require("./decode");
async function decodeAsync(streamLike, options = decode_1.defaultDecodeOptions) {
    const stream = (0, stream_1.ensureAsyncIterable)(streamLike);
    const decoder = new Decoder_1.Decoder(options.extensionCodec, options.context, options.maxStrLength, options.maxBinLength, options.maxArrayLength, options.maxMapLength, options.maxExtLength);
    return decoder.decodeAsync(stream);
}
exports.decodeAsync = decodeAsync;
function decodeArrayStream(streamLike, options = decode_1.defaultDecodeOptions) {
    const stream = (0, stream_1.ensureAsyncIterable)(streamLike);
    const decoder = new Decoder_1.Decoder(options.extensionCodec, options.context, options.maxStrLength, options.maxBinLength, options.maxArrayLength, options.maxMapLength, options.maxExtLength);
    return decoder.decodeArrayStream(stream);
}
exports.decodeArrayStream = decodeArrayStream;
function decodeMultiStream(streamLike, options = decode_1.defaultDecodeOptions) {
    const stream = (0, stream_1.ensureAsyncIterable)(streamLike);
    const decoder = new Decoder_1.Decoder(options.extensionCodec, options.context, options.maxStrLength, options.maxBinLength, options.maxArrayLength, options.maxMapLength, options.maxExtLength);
    return decoder.decodeStream(stream);
}
exports.decodeMultiStream = decodeMultiStream;
/**
 * @deprecated Use {@link decodeMultiStream()} instead.
 */
function decodeStream(streamLike, options = decode_1.defaultDecodeOptions) {
    return decodeMultiStream(streamLike, options);
}
exports.decodeStream = decodeStream;
//# sourceMappingURL=decodeAsync.js.map