import { loadToken } from "../../../../services/authService";
import { useState } from "react";

import validateDocumentUploaded from "../../../../requests/validateDocumentUpload";
import uploadFileToS3 from "../../../../requests/uploadFileToS3";
import getUploadLink from "../../../../requests/getUploadLink";

const UploadDocumentModal = ({ closeModal, directoryId }) => {
  const [loading, setLoading] = useState(false);
  const [fields, setFields] = useState({ file: null });
  const [errors, setErrors] = useState({});
  const [requestError, setRequestError] = useState(null);

  const onInput = (e) =>
    setFields({
      ...fields,
      [e.target.name]: e.target.value,
    });

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (loading) return;

    if (e.target.file.files.length === 0 || fields.file === null) {
      setErrors({
        ...errors,
        file: ["Select a file"],
      });
      return;
    }

    setErrors({});
    setLoading(true);

    const token = loadToken();

    const file = e.target.file.files[0];

    const getUploadLinkResponse = await getUploadLink(token);

    if (getUploadLinkResponse.success === false) {
      setRequestError("error getting upload link");
      setLoading(false);
      return;
    }

    var uploadResponse = await uploadFileToS3(
      getUploadLinkResponse.message.uploadUrl,
      file
    );

    if (uploadResponse.success === false) {
      setRequestError("error uploading file to s3");
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
      setRequestError("Error validating file upload");
      setLoading(false);
      return;
    }

    setLoading(false);

    window.location.reload();
  };

  return (
    <div>
      <h2>Upload Document</h2>

      <br />

      <form onSubmit={handleSubmit}>
        <div>
          <label class="form">Directory Name</label>

          <input
            type="file"
            name="file"
            className={`form ${errors.hasOwnProperty("file") ? "error" : ""}`}
            value={fields.file}
            onChange={onInput}
          />
          {errors.hasOwnProperty("file") && (
            <span class="form">{errors.file[0]}</span>
          )}
        </div>

        {/* <div>
          <input
            type="file"
            name="file"
            id=""
            onChange={(e) => {
              setFilePath(e.target.value);
            }}
          />
        </div> */}

        {!!requestError && <span className="form">{requestError}</span>}

        {loading && <p>Loading...</p>}

        <button type="submit" class="form">
          Upload
        </button>
      </form>
    </div>
  );
};

export default UploadDocumentModal;
