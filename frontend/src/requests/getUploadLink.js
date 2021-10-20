import { API_BASE_URL } from "./constants";

const getUploadLink = (token) =>
  new Promise((resolve) => {
    var myHeaders = new Headers();

    myHeaders.append("authorization", token);
    myHeaders.append("Content-Type", "application/json");

    var requestOptions = {
      method: "GET",
      headers: myHeaders,
      redirect: "follow",
    };

    const url = API_BASE_URL + `document-service/api/document/upload`;

    fetch(url, requestOptions)
      .then((response) => {
        if (response.status !== 200) {
          resolve({
            success: false,
            message: response.status,
          });
        }

        return response.json();
      })
      .then((response) => {
        resolve({
          success: true,
          message: response,
        });
      })
      .catch((error) => {
        console.log("error", error);

        resolve({
          success: false,
          message: 500,
        });
      });
  });

export default getUploadLink;
