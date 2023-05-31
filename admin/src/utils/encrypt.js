import AES from "crypto-js/aes";
import CryptoJS from "crypto-js";
import Cookies from "js-cookie";

/**
 * 生成密钥字节数组, 原始密钥字符串不足128位, 补填0.
 * @param {string} key - 原始 key 值
 * @return Buffer
 */
const fillKey = (key) => {
  const filledKey = Buffer.alloc(256 / 8);
  const keys = Buffer.from(key);
  // 如果 key 比较短，就循环 key, 否则循环 buffer
  if (keys.length < filledKey.length) {
    keys.map((b, i) => (filledKey[i] = keys[i]));
  } else {
    filledKey.map((b, i) => (filledKey[i] = keys[i]));
  }

  return filledKey;
};

export function GetKey() {
  const csrfToken = Cookies.get("XSRF-TOKEN");
  if (csrfToken) {
    return CryptoJS.SHA256(csrfToken).toString();
  } else {
    // .......
    console.error("no xsrf token found");
    throw new Error("No csrf token found");
  }
}

export function EncryptSecret(strToEncrypt, nonce) {
  return AES.encrypt(strToEncrypt, CryptoJS.enc.Utf8.parse(fillKey(nonce)), {
    mode: CryptoJS.mode.ECB,
    padding: CryptoJS.pad.Pkcs7,
  }).toString();
}
