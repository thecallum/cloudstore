import Modal from "react-modal";
import { useState, useEffect } from "react";

import CreateDirectoryModal from "./modals/createDirectoryModal";
import UploadDocumentModal from "./modals/uploadDocumentModal";

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

const TaskBar = ({ directoryId }) => {
  const [selectedModal, setSelectedModal] = useState(null);

  const handleCustomEvent = (e) => {
    // console.log("custom event", e.detail.modalName);
    openModal(e.detail.modalName);
  };

  useEffect(() => {
    window.addEventListener("open-modal", handleCustomEvent);

    return function cleanup() {
      window.removeEventListener("open-modal", handleCustomEvent);
    };
  }, []);

  function openModal(modalName) {
    setSelectedModal(modalName);
  }

  function closeModal() {
    setSelectedModal(null);
  }

  const modalList = [
    { name: "createDirectory", label: "Create Directory" },
    { name: "uploadDocument", label: "Upload Document" },
  ];

  return (
    <div
      style={{
        border: "1px solid black",
        padding: "15px",
      }}
    >
      <h2>Taskbar</h2>

      <ul>
        {modalList.map((x, index) => (
          <li key={index}>
            <button type="button" onClick={() => openModal(x.name)}>
              {x.label}
            </button>
          </li>
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
    </div>
  );
};

export default TaskBar;
