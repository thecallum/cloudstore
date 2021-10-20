import styled from "styled-components";
import { Link } from "react-router-dom";

const StyledDocument = styled.li`
  margin: 0;
  padding: 0;
  border-radius: 3px;
  border: 1px solid #ccc;
  padding: 15px;
  cursor: pointer;

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
`;

const StyledDocumentBottom = styled.div``;

const Document = ({ document }) => {
  return (
    <StyledDocument>
      <StyledDocumentTop />

      <StyledDocumentBottom>{document.name}</StyledDocumentBottom>
    </StyledDocument>
  );
};

export default Document;
