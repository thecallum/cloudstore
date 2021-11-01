import Modal from "react-modal";
import { useState, useEffect } from "react";
import fileSize from "filesize.js";

import getDocumentDownloadLinkRequest from "../../../../requests/getDocumentDownloadLink";
import { loadToken } from "../../../../services/authService";

import deleteDocumentRequest from "../../../../requests/deleteDocument";

const customStyles = {
  content: {
    top: "50%",
    left: "50%",
    right: "auto",
    bottom: "auto",
    marginRight: "-50%",
    transform: "translate(-50%, -50%)",
  },
};

Modal.setAppElement("#root");

const ModalContents = ({ document = null }) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const getDownloadLink = async () => {
    const token = loadToken();

    if (loading) return;

    setLoading(true);
    setError(null);

    const response = await getDocumentDownloadLinkRequest(token, document.id);

    console.log({ response });

    setLoading(false);

    if (response.success === false) {
      setError("Unable to download document");
    } else {
      // open in new tab
      const link = response.message.documentLink;
      window.open(link, "_blank").focus();
    }
  };

  const handleDeleteDocument = async () => {
    const status = window.confirm(
      "Are you sure you want to delete this document?"
    );

    if (status !== true) return;

    if (loading) return;

    setLoading(true);
    setError(null);

    const token = loadToken();

    var response = await deleteDocumentRequest(token, document.id);

    if (response.success === false) {
      setError("Unable to delete document");
    } else {
      window.location.reload();
    }

    setLoading(false);
  };

  if (document === null) return null;

  return (
    <>
      <h2>Preview Document</h2>

      <dl>
        <dt>Name</dt>
        <dd>{document.name}</dd>

        <dt>FileSize</dt>
        <dd>{fileSize(document.fileSize)}</dd>
      </dl>

      <br />

      {!!error && <span className="form">{error}</span>}

      {loading && <p>Loading...</p>}

      <button type="button" class="form" onClick={getDownloadLink}>
        Download Document
      </button>

      <button type="button" class="form danger" onClick={handleDeleteDocument}>
        Delete Document
      </button>
    </>
  );
};

const PreviewDocumentModal = ({ documents = [] }) => {
  const [showModal, setShowModal] = useState(false);
  const [currentDocument, setCurrentDocument] = useState(null);

  const showDocumentPreview = (e) => {
    const documentId = e.detail.documentId;

    setCurrentDocument(documents.filter((x) => x.id === documentId)[0]);

    openModal();
  };

  useEffect(() => {
    window.addEventListener("show-document-preview", showDocumentPreview);

    return function cleanup() {
      window.removeEventListener("show-document-preview", showDocumentPreview);
    };
  }, []);

  function openModal() {
    setShowModal(true);
  }

  function closeModal() {
    setShowModal(false);
  }

  return (
    <div>
      <Modal
        isOpen={showModal}
        onRequestClose={closeModal}
        style={customStyles}
        contentLabel="Example Modal"
      >
        <ModalContents document={currentDocument} />
      </Modal>
    </div>
  );
};

export default PreviewDocumentModal;
