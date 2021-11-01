import Modal from "react-modal";
import { useState, useEffect } from "react";
import styled from "styled-components";

import {
  createDirectoryAction,
  uploadDocumentAction,
  deleteDirectoryAction,
  renameDirectoryAction,
} from "./modalActions";

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

const StyledList = styled.ul`
  margin: -5px -5px;
  padding: 0;
  display: flex;
  flex-wrap: wrap;
`;

const StyledListItem = styled.li`
  margin: 5px 5px;
  display: block;
  padding: 0;

  button {
    border-radius: 0;
    border: none;
    padding: 4px 8px;
    cursor: pointer;
    background: #ddd;
  }
`;

const ActionItem = ({ text, name, openModal }) => {
  return (
    <StyledListItem>
      <button type="button" onClick={() => openModal(name)}>
        {text}
      </button>
    </StyledListItem>
  );
};

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

  const openModal = (modalName) => setSelectedModal(modalName);
  const closeModal = () => setSelectedModal(null);

  const actionList = [createDirectoryAction, uploadDocumentAction];

  if (directory !== null) {
    actionList.push(deleteDirectoryAction);
    actionList.push(renameDirectoryAction);
  }

  return (
    <div
      style={{
        border: "1px solid black",
        padding: "15px",
      }}
    >
      <StyledList>
        {actionList.map((x, index) => (
          <ActionItem
            text={x.text}
            name={x.name}
            openModal={openModal}
            key={index}
          />
        ))}
      </StyledList>

      {/* Render Modals  */}

      {actionList.map((x, index) => {
        if (selectedModal !== x.name) return null;

        return (
          <Modal
            key={index}
            isOpen={true}
            onRequestClose={closeModal}
            style={customStyles}
          >
            <x.component
              closeModal={closeModal}
              directoryId={directoryId}
              directory={directory}
            />
          </Modal>
        );
      })}
    </div>
  );
};

export default TaskBar;
