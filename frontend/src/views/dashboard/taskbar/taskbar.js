import Modal from "react-modal";
import { useState, useEffect } from "react";

import CreateDirectoryModal from "./modals/createDirectoryModal";
import UploadDocumentModal from "./modals/uploadDocumentModal";
import DeleteDirectoryModal from "./modals/deleteDirectoryModal";
import RenameDirectoryModal from "./modals/renameDirectoryModal";

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

const modalActions = {
  CREATE_DIRECTORY_MODAL: "CREATE_DIRECTORY_MODAL",
  UPLOAD_DOCUMENT_MODAL: "UPLOAD_DOCUMENT_MODAL",
  DELETE_DIRECTORY_MODAL: "DELETE_DIRECTORY_MODAL",
  RENAME_DIRECTORY_MODAL: "RENAME_DIRECTORY_MODAL",
};

Modal.setAppElement("#root");

const TaskBar = ({ directoryId, directory = null }) => {
  const [selectedModal, setSelectedModal] = useState(null);

  const handleCustomEvent = (e) => {
    openModal(e.detail.modalName);
  };

  useEffect(() => {
    window.addEventListener("open-modal", handleCustomEvent);

    return function cleanup() {
      window.removeEventListener("open-modal", handleCustomEvent);
    };
  }, [directoryId, directory]);

  function openModal(modalName) {
    setSelectedModal(modalName);
  }

  function closeModal() {
    setSelectedModal(null);
  }

  const actionList = [];

  // Add Modal actions
  const createDirectoryAction = {
    component: (
      <button
        type="button"
        onClick={() => openModal(modalActions.CREATE_DIRECTORY_MODAL)}
      >
        Create Directory
      </button>
    ),
  };

  const uploadDocumentAction = {
    component: (
      <button
        type="button"
        onClick={() => openModal(modalActions.UPLOAD_DOCUMENT_MODAL)}
      >
        Upload Document
      </button>
    ),
  };

  actionList.push(createDirectoryAction);
  actionList.push(uploadDocumentAction);

  if (directory !== null) {
    const deleteDirectoryAction = {
      component: (
        <button
          type="button"
          onClick={() => openModal(modalActions.DELETE_DIRECTORY_MODAL)}
        >
          Delete Directory
        </button>
      ),
    };

    actionList.push(deleteDirectoryAction);

    const renameDirectoryModal = {
      component: (
        <button
          type="button"
          onClick={() => openModal(modalActions.RENAME_DIRECTORY_MODAL)}
        >
          Rename Directory
        </button>
      ),
    };

    actionList.push(renameDirectoryModal);
  }

  return (
    <div
      style={{
        border: "1px solid black",
        padding: "15px",
      }}
    >
      <h2>Taskbar</h2>

      <ul>
        {actionList.map((x, index) => (
          <li key={index}>{x.component}</li>
        ))}
      </ul>

      {selectedModal === modalActions.CREATE_DIRECTORY_MODAL && (
        <Modal
          isOpen={true}
          onRequestClose={closeModal}
          style={customStyles}
          contentLabel="Example Modal"
        >
          <CreateDirectoryModal directoryId={directoryId} />
        </Modal>
      )}

      {selectedModal === modalActions.UPLOAD_DOCUMENT_MODAL && (
        <Modal
          isOpen={true}
          onRequestClose={closeModal}
          style={customStyles}
          contentLabel="Example Modal"
        >
          <UploadDocumentModal directoryId={directoryId} />
        </Modal>
      )}

      {selectedModal === modalActions.DELETE_DIRECTORY_MODAL && (
        <Modal
          isOpen={true}
          onRequestClose={closeModal}
          style={customStyles}
          contentLabel="Example Modal"
        >
          <DeleteDirectoryModal
            closeModal={closeModal}
            directoryId={directoryId}
          />
        </Modal>
      )}

      {selectedModal === modalActions.RENAME_DIRECTORY_MODAL && (
        <Modal
          isOpen={true}
          onRequestClose={closeModal}
          style={customStyles}
          contentLabel="Example Modal"
        >
          <RenameDirectoryModal closeModal={closeModal} directory={directory} />
        </Modal>
      )}
    </div>
  );
};

export default TaskBar;
