import styled from "styled-components";
import { Link } from "react-router-dom";

const StyledDocument = styled.button`
  margin: 0;
  padding: 0;
  border-radius: 3px;
  border: 1px solid #ccc;
  padding: 15px;
  cursor: pointer;
  background: #fff;

  display: flex;
  flex-direction: column;
  justify-content: flex-start;

  &:hover {
    background: #f9f9f9;
  }
`;

const StyledDocumentTop = styled.div`
  background: #ddd;
  height: 100px;
  margin-bottom: 15px;
  width: 100%;
`;

const StyledDocumentBottom = styled.div``;

const Document = ({ document }) => {
  const showDocumentPreview = () => {
    const event = new CustomEvent("show-document-preview", {
      detail: {
        documentId: document.id,
      },
    });

    window.dispatchEvent(event);
  };

  return (
    <StyledDocument onClick={showDocumentPreview}>
      <StyledDocumentTop />

      <StyledDocumentBottom>{document.name}</StyledDocumentBottom>
    </StyledDocument>
  );
};

export default Document;
