import React from "react";
import { useEffect, useState } from "react";
import getAllDocuments from "../../requests/getAllDocuments";
import getAllDirectories from "../../requests/getAllDirectories";

import Layout from "../layout/layout";

import { useLocation } from "react-router-dom";
import { loadToken } from "../../services/authService";

import CreateDirectory from "./createDirectory";
import UploadDocument from "./uploadDocument";
import DashboardBreadcrumb from "./breadcrumb";
import DirectoriesList from "./directoriesList";
import DocumentsList from "./documentsList";

const decodeDashboardPath = () => {
  const urlComponents = window.location.pathname.split("/").slice(2);

  if (urlComponents.length === 1 && urlComponents[0] === "") {
    return [];
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

const Dashboard = (props) => {
  const [loading, setLoading] = useState(true);
  const [documents, setDocuments] = useState([]);
  const [directories, setDirectories] = useState([]);

  const location = useLocation();

  const loadAll = (urlComponents) => {
    setLoading(true);

    const directoryId =
      urlComponents.length === 0
        ? null
        : urlComponents[urlComponents.length - 1].name;

    const token = loadToken();

    Promise.all([getAllDocuments(token, directoryId), getAllDirectories(token)])
      .then((res) => {
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
    loadAll(urlComponents);
  }, [location.key]);

  return (
    <Layout>
      <h1>Dashboard Component: </h1>

      <CreateDirectory
        directoryId={
          urlComponents.length === 0
            ? null
            : urlComponents[urlComponents.length - 1].name
        }
      />

      <UploadDocument
        directoryId={
          urlComponents.length === 0
            ? null
            : urlComponents[urlComponents.length - 1].name
        }
      />

      {loading === true ? (
        <>
          <p>Loading...</p>
        </>
      ) : (
        <>
          <DashboardBreadcrumb
            urlComponents={urlComponents}
            directories={directories}
          />

          <h2>
            Current Directory: [
            {urlComponents.length === 0
              ? "Home"
              : directories.filter(
                  (x) =>
                    x.directoryId ===
                    urlComponents[urlComponents.length - 1].name
                )[0].name}
            ]
          </h2>

          <DirectoriesList
            directories={directories}
            urlComponents={urlComponents}
          />

          <DocumentsList documents={documents} urlComponents={urlComponents} />
        </>
      )}
    </Layout>
  );
};

export default Dashboard;
