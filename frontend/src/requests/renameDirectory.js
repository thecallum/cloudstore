import { API_BASE_URL } from "./constants";

const renameDirectory = (token, directoryId, newName) =>
  new Promise((resolve) => {
    var headers = new Headers();
    headers.append("Content-Type", "application/json");

    let url = API_BASE_URL + `api/directory/${directoryId}`;

    headers.append("authorization", token);

    var body = JSON.stringify({
      name: newName,
    });

    var requestOptions = {
      method: "PATCH",
      headers: headers,
      redirect: "follow",
      body: body,
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

export default renameDirectory;
