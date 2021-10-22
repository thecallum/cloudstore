import { loadToken } from "../../../../services/authService";
import { useState } from "react";

import validateDocumentUploaded from "../../../../requests/validateDocumentUpload";
import uploadFileToS3 from "../../../../requests/uploadFileToS3";
import getUploadLink from "../../../../requests/getUploadLink";

const UploadDocumentModal = ({ closeModal, directoryId }) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (loading) return;

    if (e.target.file.files.length === 0 || filePath === null) {
      setError("Select a file");
      return;
    }

    setLoading(true);

    setError(null);

    const token = loadToken();

    const file = e.target.file.files[0];

    const getUploadLinkResponse = await getUploadLink(token);

    if (getUploadLinkResponse.success === false) {
      setError("error getting upload link");
      setLoading(false);
      return;
    }

    var uploadResponse = await uploadFileToS3(
      getUploadLinkResponse.message.uploadUrl,
      file
    );

    if (uploadResponse.success === false) {
      setError("error uploading file to s3");
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
      setError("Error validating file upload");
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

        {!!error && <p style={{ color: "hsl(0, 50%, 50%)" }}>{error}</p>}

        <div>
          <button type="submit">Upload</button>
        </div>
      </form>
    </div>
  );
};

export default UploadDocumentModal;
