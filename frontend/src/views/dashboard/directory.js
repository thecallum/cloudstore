import styled from "styled-components";
import { Link } from "react-router-dom";

const StyledDirectory = styled.li`
  display: block;
  margin: 0;
  padding: 0;
  border-radius: 3px;
  border: 1px solid #ccc;

  a {
    text-decoration: none;
    color: inherit;
    display: block;
    padding: 15px;
  }

  &:hover {
    background: #f9f9f9;
  }
`;

const Directory = ({ directory, urlComponents }) => {
  let directoryUrl = `/dashboard`;

  if (urlComponents.length > 0) {
    directoryUrl += urlComponents[urlComponents.length - 1].full;
  }

  directoryUrl += `/` + directory.id;

  return (
    <StyledDirectory>
      <Link to={directoryUrl}>{directory.name}</Link>
    </StyledDirectory>
  );
};

export default Directory;
