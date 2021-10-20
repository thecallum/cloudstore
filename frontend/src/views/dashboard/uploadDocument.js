import { useState } from "react";
import { loadToken } from "../../services/authService";

import validateDocumentUploaded from "../../requests/validateDocumentUpload";
import uploadFileToS3 from "../../requests/uploadFileToS3";
import getUploadLink from "../../requests/getUploadLink";

const UploadDocument = ({ directoryId = null }) => {
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (loading) return;

    if (e.target.file.files.length === 0) return;
    if (filePath === null) return;

    setLoading(true);

    const token = loadToken();

    const file = e.target.file.files[0];

    const getUploadLinkResponse = await getUploadLink(token);

    if (getUploadLinkResponse.success === false) {
      alert("error getting upload link");
      setLoading(false);
      return;
    }

    var uploadResponse = await uploadFileToS3(
      getUploadLinkResponse.message.uploadUrl,
      file
    );

    if (uploadResponse.success === false) {
      alert("error uploading file to s3");
      setLoading(false);
      return;
    }

    var validateResponse = await validateDocumentUploaded(
      file.name,
      getUploadLinkResponse.message.documentId,
      token,
      directoryId
    );

    if (validateResponse.success === false) {
      alert("Error validating file upload");
      setLoading(false);
      return;
    }

    setLoading(false);

    window.location.reload();
  };

  const [filePath, setFilePath] = useState(null);

  return (
    <div style={{ border: "1px solid black", padding: "15px" }}>
      <h2>Upload Document</h2>

      <p>FilePath: {filePath}</p>

      <form onSubmit={handleSubmit}>
        <div>
          <input
            type="file"
            name="file"
            id=""
            onChange={(e) => {
              setFilePath(e.target.value);
            }}
          />
        </div>

        {loading && <p>Loading...</p>}

        <div>
          <button type="submit">Upload</button>
        </div>
      </form>
    </div>
  );
};

export default UploadDocument;
