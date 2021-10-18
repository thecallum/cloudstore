import { API_BASE_URL } from "./constants";
import { loadToken } from "../services/authService";

var headers = new Headers();
headers.append("Content-Type", "application/json");

const getAllDocuments = (token, directoryId = null) =>
  new Promise((resolve) => {
    let url = API_BASE_URL + "document-service/api/document/";

    if (!!directoryId !== false) url += `?directoryId=${directoryId}`;

    headers.append("authorization", token);

    var requestOptions = {
      method: "GET",
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

        return res.json();
      })
      .then((res) => {
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

export default getAllDocuments;
