import { loadToken } from "../../../../services/authService";
import deleteDirectoryRequest from "../../../../requests/deleteDirectory";
import { useState } from "react";
import { useHistory } from "react-router-dom";

const DeleteDirectoryModal = ({ directoryId, closeModal }) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const history = useHistory();

  const handleSubmit = (e) => {
    e.preventDefault();
    if (loading) return;

    setLoading(true);

    const token = loadToken();
    deleteDirectoryRequest(token, directoryId)
      .then((res) => {
        if (!res.success) {
          // do nothing
          console.log({ res });
          setError("Something went wrong");
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

      {loading && <p>Loading...</p>}

      <form onSubmit={handleSubmit}>
        {!!error && <p style={{ color: "hsl(0, 50%, 50%)" }}>{error}</p>}

        <p> Are you sure you want to delete this directory?</p>

        <div>
          <button type="submit">Delete</button>

          <button type="button" onClick={closeModal}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
};

export default DeleteDirectoryModal;
