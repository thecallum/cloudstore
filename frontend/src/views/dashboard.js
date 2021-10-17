import React from "react";
import { useEffect, useState } from "react";

import {
  BrowserRouter,
  Switch,
  Route,
  Link,
  useLocation,
} from "react-router-dom";

const decodeDashboardPath = () => {
  const urlComponents = window.location.pathname.split("/").slice(2);

  if (urlComponents.length === 1 && urlComponents[0] === "") {
    return [{ base: "/", full: "/dashboard/" }];
  }

  const formattedUrls = [];

  for (var i = 0; i < urlComponents.length; i++) {
    // calculate full url based on previous full url + baseUrl

    var newUrl = {
      name: urlComponents[i],
      base: `/${urlComponents[i]}`,
    };

    if (i === 0) {
      newUrl.full = newUrl.base;
    } else {
      var previousIndex = i - 1;
      newUrl.full = formattedUrls[previousIndex].full + newUrl.base;
    }

    formattedUrls.push(newUrl);
  }

  return formattedUrls;
};

const DashboardBreadcrumb = ({ urlComponents }) => {
  return (
    <ul
      style={{
        margin: 0,
        padding: 0,
        display: "flex",
        // background: "orange",
        padding: "5px",
        marginLeft: "20px",
      }}
    >
      {urlComponents.map((x) => (
        <li
          style={{
            display: "block",
            marginRight: 2,
          }}
        >
          <Link to={`/dashboard${x.full}`}>{x.base}</Link>
        </li>
      ))}
    </ul>
  );
};

export default (props) => {
  const [path, setPath] = useState(null);
  const [loading, setLoading] = useState(true);

  const location = useLocation();

  useEffect(() => {
    console.log("location changed");

    setLoading(true);
    setPath(window.location.pathname);

    setTimeout(() => {
      setLoading(false);
    }, 200);
  }, [location.key]);

  const urlComponents = decodeDashboardPath();

  return (
    <div>
      <DashboardBreadcrumb urlComponents={urlComponents} />

      <p>{path}</p>

      {loading === true ? (
        <>
          <p>Loading...</p>
        </>
      ) : (
        <>
          <h1>Dashboard Component</h1>
        </>
      )}
    </div>
  );
};
