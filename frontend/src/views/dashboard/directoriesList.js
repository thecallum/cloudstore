import styled from "styled-components";

import Directory from "./directory";

const StyledDirectoriesList = styled.ul`
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

const DirectoriesList = ({ directories, urlComponents }) => {
  let filteredDirectories;

  if (urlComponents.length === 0) {
    filteredDirectories = directories.filter(
      (x) => x.userId === x.parentDirectoryId
    );
  } else {
    filteredDirectories = directories.filter(
      (x) =>
        x.parentDirectoryId === urlComponents[urlComponents.length - 1].name
    );
  }

  if (filteredDirectories.length === 0) return null;

  return (
    <>
      <h2>Directories</h2>

      <StyledDirectoriesList>
        {filteredDirectories.map((x, index) => (
          <Directory directory={x} urlComponents={urlComponents} key={index} />
        ))}
      </StyledDirectoriesList>
    </>
  );
};

export default DirectoriesList;
