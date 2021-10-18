import { Link } from "react-router-dom";

const links = [
  {
    name: "Dashboard",
    url: "/dashboard/",
  },
  {
    name: "Account",
    url: "/account/",
  },
  {
    name: "Login",
    url: "/login/",
  },
];

const header = () => {
  return (
    <header
      style={{
        background: "hsl(180, 20%, 50%)",
        padding: "30px",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
      }}
    >
      <div>CloudStore</div>

      <ul
        style={{
          margin: 0,
          padding: 0,
          display: "flex",
        }}
      >
        {links.map((x) => (
          <li
            style={{
              display: "block",
              marginLeft: "8px",
            }}
          >
            <Link to={x.url}>{x.name}</Link>
          </li>
        ))}
      </ul>
    </header>
  );
};

export default header;
