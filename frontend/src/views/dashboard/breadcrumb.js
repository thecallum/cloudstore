import { Link } from "react-router-dom";

const Ul = ({ children }) => (
  <ul
    style={{
      margin: "15px 0 10px",
      padding: 0,
      display: "flex",
      flexDirection: "row",
      flexWrap: "wrap",
    }}
  >
    {children}
  </ul>
);

const DashboardBreadcrumb = ({ urlComponents, directories }) => {
  const lastIndex = urlComponents.length - 1;

  return (
    <>
      <Ul>
        <li
          style={{
            display: "block",
            marginRight: 2,
          }}
        >
          <Link to="/dashboard/">/</Link>
        </li>
        {urlComponents.map((x, index) => (
          <li
            key={index}
            style={{
              display: "block",
              marginRight: 2,
            }}
          >
            {index === lastIndex ? (
              <>{directories.filter((d) => d.id === x.name)[0]["name"]}/</>
            ) : (
              <Link to={`/dashboard${x.full}`}>
                {directories.filter((d) => d.id === x.name)[0].name}/
              </Link>
            )}
          </li>
        ))}
      </Ul>
    </>
  );
};

export default DashboardBreadcrumb;
