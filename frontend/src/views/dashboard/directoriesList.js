import { Link } from "react-router-dom";

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

  return (
    <>
      <h2>Directories [{filteredDirectories.length}]</h2>

      <ul>
        {filteredDirectories.map((x, index) => {
          let directoryUrl = `/dashboard`;

          if (urlComponents.length > 0) {
            directoryUrl += urlComponents[urlComponents.length - 1].full;
          }

          directoryUrl += `/` + x.directoryId;

          return (
            <li key={index}>
              <Link to={directoryUrl}>{x.name}</Link>
            </li>
          );
        })}
      </ul>
    </>
  );
};

export default DirectoriesList;
