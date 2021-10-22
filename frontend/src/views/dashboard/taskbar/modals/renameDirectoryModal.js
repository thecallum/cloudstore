import { loadToken } from "../../../../services/authService";
import renameDirectoryRequest from "../../../../requests/renameDirectory";
import { useState, useEffect } from "react";

const RenameDirectoryModal = ({ directory, closeModal }) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const originalName = directory.name;

  const [name, setName] = useState(originalName);

  const handleSubmit = (e) => {
    e.preventDefault();
    if (loading) return;

    if (nameChanged === false) return;

    if (name === "") {
      setError("Directory name cannot be empty");
      return;
    }

    setLoading(true);
    setError(null);

    const token = loadToken();
    renameDirectoryRequest(token, directory.directoryId, name)
      .then((res) => {
        if (!res.success) {
          // do nothing

          setError("Something went wrong");
          return;
        }

        window.location.reload();
      })
      .finally(() => {
        setLoading(false);
      });
  };

  const nameChanged = name !== originalName;

  return (
    <div>
      <h2>Rename Directory</h2>

      {loading && <p>Loading...</p>}

      <form onSubmit={handleSubmit}>
        <div>
          <div>
            <label htmlFor="">Name</label>
          </div>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>

        {!!error && <p style={{ color: "hsl(0, 50%, 50%)" }}>{error}</p>}

        <div>
          <button type="submit" disabled={nameChanged === false}>
            Update
          </button>
        </div>
      </form>
    </div>
  );
};

export default RenameDirectoryModal;
