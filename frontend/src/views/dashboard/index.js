import React from "react";
import { useEffect, useState } from "react";
import getAllDocuments from "../../requests/getAllDocuments";
import getAllDirectories from "../../requests/getAllDirectories";
import getStorageUsage from "../../requests/getStorageUsage";
import { Link } from "react-router-dom";

import StorageUsage from "./storageUsage";

import Layout from "../layout/layout";

import { useLocation } from "react-router-dom";
import { loadToken } from "../../services/authService";

import DashboardBreadcrumb from "./breadcrumb";
import DirectoriesList from "./directoriesList";
import DocumentsList from "./documentsList";

import TaskBar from "./taskbar/taskbar";
import PreviewDocumentModal from "./taskbar/modals/previewDocumentModal";

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

const DashboardLayout = ({ children }) => {
  return (
    <Layout>
      <h1>Dashboard</h1>

      {children}
    </Layout>
  );
};

const Dashboard = (props) => {
  const [loading, setLoading] = useState(true);
  const [documents, setDocuments] = useState([]);
  const [directories, setDirectories] = useState([]);
  const [storageUsage, setStorageUsage] = useState(null);

  const [directoryId, setDirectoryId] = useState(null);
  const [directory, setDirectory] = useState(null);

  const [directoryNotFound, setDirectoryNotFound] = useState(false);

  const location = useLocation();

  const loadAll = (urlComponents) => {
    setLoading(true);

    let directoryId = null;

    if (urlComponents.length > 0) {
      directoryId = urlComponents[urlComponents.length - 1].name;
    }

    setDirectoryId(directoryId);

    const token = loadToken();

    const taskList = [
      getAllDocuments(token, directoryId),
      getAllDirectories(token),
      getStorageUsage(token),
    ];

    Promise.all(taskList)
      .then((res) => {
        const [documentsResponse, directoriesResponse, storageUsageResponse] =
          res;

        if (documentsResponse.success) {
          setDocuments(documentsResponse.message.documents);
        }

        let directories = [];
        let directory = null;

        // default value
        setDirectoryNotFound(false);

        if (directoriesResponse.success) {
          directories = directoriesResponse.message.directories;

          if (directoryId !== null) {
            directory = directories.filter(
              (x) => x.directoryId === directoryId
            )[0];

            if (directory === undefined) setDirectoryNotFound(true);
          }
        }
        setDirectories(directories);

        setDirectory(directory);

        if (storageUsageResponse.success === true) {
          setStorageUsage(storageUsageResponse.message);
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

  if (loading) {
    return (
      <DashboardLayout>
        <p>Loading...</p>
      </DashboardLayout>
    );
  }

  if (directoryNotFound) {
    return (
      <DashboardLayout>
        <h2>Directory Not Found</h2>

        <p>
          <Link to="/dashboard/">Back to Safety</Link>
        </p>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout>
      <TaskBar directoryId={directoryId} directory={directory} />

      {/* <pre>{JSON.stringify(directory, null, 2)}</pre>

<pre>{JSON.stringify(directoryId, null, 2)}</pre> */}
      {/* 
          <DashboardBreadcrumb
            urlComponents={urlComponents}
            directories={directories}
          /> */}

      <StorageUsage storageUsage={storageUsage} />

      <h2>Current Directory: [{directory?.name}]</h2>

      <PreviewDocumentModal documents={documents} />

      <DirectoriesList
        directories={directories}
        urlComponents={urlComponents}
      />

      <DocumentsList documents={documents} urlComponents={urlComponents} />
    </DashboardLayout>
  );
};

export default Dashboard;
