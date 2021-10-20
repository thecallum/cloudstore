const uploadFileToS3 = (presignedUrl, filePath) =>
  new Promise((resolve) => {
    var formdata = new FormData();
    // formdata.append("file", fileInput.files[0], filePath);

    var requestOptions = {
      method: "PUT",
      body: formdata,
      redirect: "follow",
      body: filePath,
    };

    fetch(presignedUrl, requestOptions)
      .then((response) => {
        if (response.status !== 200) {
          resolve({
            success: false,
            message: response.status,
          });
        }

        return response.text();
      })
      .then((response) => {
        resolve({
          success: true,
          //   message: response,
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

export default uploadFileToS3;
