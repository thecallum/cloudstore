import styled from "styled-components";

import Document from "./document";

const StyledDocumentsList = styled.ul`
  margin: 0;
  padding: 0;

  display: grid;

  grid-template-columns: 1fr;
  grid-column-gap: 15px;
  grid-row-gap: 15px;

  @media (min-width: 500px) {
    grid-template-columns: 1fr 1fr;
  }

  @media (min-width: 700px) {
    grid-template-columns: 1fr 1fr 1fr;
  }
`;

const DocumentsList = ({ documents, urlComponents }) => {
  if (documents.length === 0) {
    return (
      <>
        <h2>No documements here..</h2>

        <button>Button to upload new document</button>
      </>
    );
  }

  return (
    <>
      <h2>Documents</h2>

      <StyledDocumentsList>
        {documents.map((x, index) => (
          <Document key={index} document={x} />
        ))}
      </StyledDocumentsList>
    </>
  );
};

export default DocumentsList;
