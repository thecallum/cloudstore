import { API_BASE_URL, AUTH_HEADER } from "./constants";

var headers = new Headers();
headers.append("Content-Type", "application/json");

const login = ({ email, password }) =>
  new Promise((resolve) => {
    const url = API_BASE_URL + "api/auth/login";

    var body = JSON.stringify({
      email,
      password,
    });

    var requestOptions = {
      method: "POST",
      headers: headers,
      body: body,
      redirect: "follow",
      mode: "cors",
    };

    fetch(url, requestOptions)
      .then((res) => {
        if (res.status !== 200) {
          resolve({
            success: false,
            status: res.status,
          });
        }

        const token = res.headers.get(AUTH_HEADER);

        resolve({
          success: true,
          message: token,
        });
      })
      .catch((error) => {
        resolve({
          success: false,
          status: 500,
        });
      });
  });

export default login;
