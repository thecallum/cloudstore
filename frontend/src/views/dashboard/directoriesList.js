import styled from "styled-components";

import Directory from "./directory";

const StyledDirectoriesList = styled.ul`
  margin: 0 0 30px;
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
  console.log({ directories, urlComponents });

  const parentDirectoryId =
    urlComponents.length === 0
      ? null
      : urlComponents[urlComponents.length - 1].name;

  const filteredDirectories = directories.filter(
    (x) => x.parentDirectoryId == parentDirectoryId
  );

  if (filteredDirectories.length === 0) return null;

  return (
    <>
      <StyledDirectoriesList>
        {filteredDirectories.map((x, index) => (
          <Directory directory={x} urlComponents={urlComponents} key={index} />
        ))}
      </StyledDirectoriesList>
    </>
  );
};

export default DirectoriesList;
