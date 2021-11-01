import { loadToken } from "../../../../services/authService";
import deleteDirectoryRequest from "../../../../requests/deleteDirectory";
import { useState } from "react";
import { useHistory } from "react-router-dom";

const DeleteDirectoryModal = ({ directoryId, closeModal }) => {
  const [loading, setLoading] = useState(false);
  const [requestError, setRequestError] = useState(null);

  const history = useHistory();

  const handleSubmit = (e) => {
    e.preventDefault();
    if (loading) return;

    setRequestError(null);
    setLoading(true);

    const token = loadToken();
    deleteDirectoryRequest(token, directoryId)
      .then((res) => {
        if (!res.success) {
          // do nothing
          console.log({ res });
          setRequestError("Something went wrong");
          return;
        }

        const directoriesInUrl = window.location.pathname
          .split("/dashboard/")[1]
          .split("/");

        directoriesInUrl.pop();

        const parentDirectoryUrl = `/dashboard/${directoriesInUrl.join("/")}`;

        closeModal();
        history.push(parentDirectoryUrl);
      })
      .finally(() => {
        setLoading(false);
      });
  };

  return (
    <div>
      <h2>Delete Directory</h2>

      <br />

      <form onSubmit={handleSubmit}>
        {!!requestError && <span className="form">{requestError}</span>}

        <p> Are you sure you want to delete this directory?</p>

        <br />

        {loading && <p>Loading...</p>}

        <div>
          <button type="submit" class="form">
            Delete
          </button>

          <button type="button" class="form danger" onClick={closeModal}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
};

export default DeleteDirectoryModal;
