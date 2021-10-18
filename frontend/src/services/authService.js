const LOCAL_STORAGE_KEY = "authtoken";

export const saveToken = (token) => {
  window.localStorage.setItem(LOCAL_STORAGE_KEY, token);
};

export const loadToken = () => {
  const token = window.localStorage.getItem(LOCAL_STORAGE_KEY);
  return token;
};

export const checkAuthStatus = () => {
  const token = loadToken();

  if (token === null) return false;

  return true;
};
