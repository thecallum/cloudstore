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
      <button type="button" onClick={() => openModal("createModal")}>
        Create Directory
      </button>
    ),
  };

  const uploadDocumentAction = {
    component: (
      <button type="button" onClick={() => openModal("uploadDocument")}>
        Upload Document
      </button>
    ),
  };

  actionList.push(createDirectoryAction);
  actionList.push(uploadDocumentAction);

  if (directory !== null) {
    const deleteDirectoryAction = {
      component: (
        <button type="button" onClick={() => openModal("deleteDirectory")}>
          Delete Directory
        </button>
      ),
    };

    actionList.push(deleteDirectoryAction);

    const renameDirectoryModal = {
      component: (
        <button type="button" onClick={() => openModal("renameDirectory")}>
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

      {selectedModal === "createDirectory" && (
        <Modal
          isOpen={true}
          onRequestClose={closeModal}
          style={customStyles}
          contentLabel="Example Modal"
        >
          <CreateDirectoryModal directoryId={directoryId} />
        </Modal>
      )}

      {selectedModal === "uploadDocument" && (
        <Modal
          isOpen={true}
          onRequestClose={closeModal}
          style={customStyles}
          contentLabel="Example Modal"
        >
          <UploadDocumentModal directoryId={directoryId} />
        </Modal>
      )}

      {selectedModal === "deleteDirectory" && (
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

      {selectedModal === "renameDirectory" && (
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
