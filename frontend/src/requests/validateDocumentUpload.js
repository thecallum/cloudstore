import { API_BASE_URL } from "./constants";

const validateDocumentUploaded = (
  fileName,
  documentId,
  token,
  directoryId = null
) =>
  new Promise((resolve) => {
    var myHeaders = new Headers();

    myHeaders.append("authorization", token);
    myHeaders.append("Content-Type", "application/json");

    var raw = JSON.stringify({
      fileName,
      directoryId,
    });

    var requestOptions = {
      method: "POST",
      headers: myHeaders,
      body: raw,
      redirect: "follow",
    };

    const url =
      API_BASE_URL + `document-service/api/document/validate/${documentId}`;

    fetch(url, requestOptions)
      .then((response) => {
        if (response.status !== 201) {
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

export default validateDocumentUploaded;
