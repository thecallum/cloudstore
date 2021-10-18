import React from "react";
import { useEffect, useState } from "react";
import getAllDocuments from "../requests/getAllDocuments";
import getAllDirectories from "../requests/getAllDirectories";

import Layout from "./layout/layout";

import { Link, useLocation } from "react-router-dom";
import { loadToken } from "../services/authService";

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
  const [documents, setDocuments] = useState([]);
  const [directories, setDirectories] = useState([]);

  const location = useLocation();

  const loadAll = (urlComponents) => {
    setLoading(true);

    const token = loadToken();

    Promise.all([
      getAllDocuments(token, urlComponents[0].name),
      getAllDirectories(token, urlComponents[0].name),
    ])
      .then((res) => {
        console.log({ res });

        const [documentsResponse, directoriesResponse] = res;

        if (documentsResponse.success === true) {
          setDocuments(documentsResponse.message.documents);
        } else {
          setDocuments([]);
        }

        if (directoriesResponse.success === true) {
          setDirectories(directoriesResponse.message.directories);
        } else {
          setDirectories([]);
        }
      })
      .finally(() => {
        setLoading(false);
      });
  };

  const urlComponents = decodeDashboardPath();

  useEffect(() => {
    // console.log("location changed");
    setPath(window.location.pathname);

    loadAll(urlComponents);
  }, [location.key]);

  return (
    <Layout>
      <h1>Dashboard Component</h1>

      <DashboardBreadcrumb urlComponents={urlComponents} />

      {loading === true ? (
        <>
          <p>Loading...</p>
        </>
      ) : (
        <>
          <h2>Directories [{directories.length}]</h2>

          {/* <pre>{JSON.stringify(directories, null, 2)}</pre> */}

          <ul>
            {directories.map((x) => (
              <li>
                <Link to={`/dashboard/${x.directoryId}`}>{x.name}</Link>
              </li>
            ))}
          </ul>

          <h2>Documents [{documents.length}]</h2>

          <ul>
            {documents.map((x) => (
              <li>{x.name}</li>
            ))}
          </ul>
        </>
      )}
    </Layout>
  );
};
