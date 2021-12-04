import { Link } from "react-router-dom";
import { checkAuthStatus } from "../../services/authService";


const HeaderLinks = () => {
    const isAuthenticated = checkAuthStatus();

    let links = [
      {
        name: "Dashboard",
        url: "/dashboard/",
      }
    ];
  
    if (isAuthenticated) {
      links = [
        ...links,
       
        {
          name: "Account",
          url: "/account/",
        },
        {
            name: "Sign Out",
            url: "/logout/",
          }
      ]
  
  
     
    } else {
    
      links = [
        ...links,
        {
          name: "Login",
          url: "/login/",
        },
        {
          name: "Register",
          url: "/register/",
        }
      ];
    }

   return (
    <ul
    style={{
      margin: 0,
      padding: 0,
      display: "flex",
    }}
  >
    {links.map((x, index) => (
      <li
        key={index}
        style={{
          display: "block",
          marginLeft: "8px",
        }}
      >
        <Link
          style={{
            color: "#0a0a0a",
          }}
          to={x.url}
        >
          {x.name}
        </Link>
      </li>
    ))}
  </ul>
   )
}

export default HeaderLinks;