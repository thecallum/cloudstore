import { API_BASE_URL } from "./constants";

const deleteDirectory = (token, directoryId) =>
  new Promise((resolve) => {
    var headers = new Headers();
    headers.append("Content-Type", "application/json");

    let url = API_BASE_URL + `document-service/api/directory/${directoryId}`;

    headers.append("authorization", token);

    var requestOptions = {
      method: "DELETE",
      headers: headers,
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
          return;
        }

        resolve({
          success: true,
        });
      })

      .catch((error) => {
        resolve({
          success: false,
          status: 500,
        });
      });
  });

export default deleteDirectory;
