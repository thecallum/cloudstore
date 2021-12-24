import { API_BASE_URL, AUTH_HEADER } from "./constants";

var headers = new Headers();
headers.append("Content-Type", "application/json");

const register = ({ firstName, lastName, email, password }) =>
  new Promise((resolve) => {
    const url = API_BASE_URL + "api/auth/register";

    var body = JSON.stringify({
      firstName,  
      lastName,
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

        resolve({
          success: true
        });
      })
      .catch((error) => {
        resolve({
          success: false,
          status: 500,
        });
      });
  });

export default register;
