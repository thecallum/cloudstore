import jwt_decode from "jwt-decode";
const LOCAL_STORAGE_KEY = "authtoken";

export const saveToken = (token) => {
  window.localStorage.setItem(LOCAL_STORAGE_KEY, token);
};

export const loadToken = () => {
  return window.localStorage.getItem(LOCAL_STORAGE_KEY);
};

const removeToken = () => {
  window.localStorage.removeItem(LOCAL_STORAGE_KEY);
};

export const checkAuthStatus = () => {
  const token = loadToken();

  if (token === null) return false;

  try {
    var decoded = jwt_decode(token);

    const tokenExpired = Date.now() >= decoded.exp * 1000;

    if (tokenExpired) {
      removeToken();
      return false;
    }

    return true;
  } catch (e) {
    return false;
  }
};
