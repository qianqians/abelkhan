"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    Object.defineProperty(o, k2, { enumerable: true, get: function() { return m[k]; } });
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __exportStar = (this && this.__exportStar) || function(m, exports) {
    for (var p in m) if (p !== "default" && !Object.prototype.hasOwnProperty.call(exports, p)) __createBinding(exports, m, p);
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.decode = exports.encode = void 0;
const Encoder_1 = require("./Encoder");
const Decoder_1 = require("./Decoder");
__exportStar(require("./Encoder"), exports);
__exportStar(require("./Decoder"), exports);
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
function encode(obj, initBuffSize) {
    return encode.encoder.encode(obj, initBuffSize);
}
exports.encode = encode;
encode.encoder = new Encoder_1.Encoder();
Object.freeze(encode);
/**
 * Decode the first MsgPack value encountered. This is just a
 * convenience function: the decoder used can be accessed via
 * decode.decoder.
 *
 * @param data The buffer to decode from.
 */
function decode(data) {
    return decode.decoder.decode(data);
}
exports.decode = decode;
decode.decoder = new Decoder_1.Decoder();
Object.freeze(decode);
//# sourceMappingURL=index.js.map