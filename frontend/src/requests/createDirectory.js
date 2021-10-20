import { API_BASE_URL } from "./constants";

const createDirectory = (token, { name, parentDirectoryId = null }) =>
  new Promise((resolve) => {
    var headers = new Headers();
    headers.append("Content-Type", "application/json");

    const url = API_BASE_URL + "document-service/api/directory";

    headers.append("authorization", token);

    var body = JSON.stringify({
      name,
      parentDirectoryId,
    });

    var requestOptions = {
      method: "POST",
      headers: headers,
      body: body,
    };

    fetch(url, requestOptions)
      .then((res) => {
        if (res.status !== 201) {
          resolve({
            success: false,
            status: res.status,
          });
        }

        resolve({
          success: true,
          message: res,
        });
      })
      .catch((error) => {
        resolve({
          success: false,
          status: 500,
        });
      });
  });

export default createDirectory;
