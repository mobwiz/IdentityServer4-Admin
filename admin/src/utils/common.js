const defaultLang = (
  navigator.browserLanguage || navigator.language
).toLowerCase();

const getLanguage = () => {
  return localStorage.getItem("language") || defaultLang;
};

export { getLanguage };
