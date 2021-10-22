import { loadToken } from "../../../../services/authService";
import createDirectoryRequest from "../../../../requests/createDirectory";
import { useState } from "react";

const CreateDirectoryModal = ({ closeModal, directoryId }) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSubmit = (e) => {
    e.preventDefault();

    if (loading) return;

    const payload = {
      name: e.target.name.value,
    };

    if (payload.name === null || payload.name === "") {
      setError("Name cannot be empty");
      return;
    }

    setLoading(true);

    if (!!directoryId) {
      payload.parentDirectoryId = directoryId;
    }

    const token = loadToken();

    createDirectoryRequest(token, payload)
      .then((res) => {
        if (!res.success) {
          // do nothing

          setError("Something went wrong");
          return;
        }

        // reload on success
        // closeModal()
        window.location.reload();
      })
      .finally(() => {
        setLoading(false);
      });
  };

  return (
    <div>
      <h2>Create Directory</h2>

      {loading && <p>Loading...</p>}

      <form onSubmit={handleSubmit}>
        <div>
          <div>
            <label htmlFor="">Name</label>
          </div>
          <input type="text" name="name" id="" />
        </div>

        {!!error && <p style={{ color: "hsl(0, 50%, 50%)" }}>{error}</p>}

        <div>
          <button type="submit">Create</button>
        </div>
      </form>
    </div>
  );
};

export default CreateDirectoryModal;
