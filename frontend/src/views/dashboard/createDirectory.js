import { useState } from "react";

import createDirectoryRequest from "../../requests/createDirectory";
import { loadToken } from "../../services/authService";

const CreateDirectory = ({ directoryId }) => {
  const [loading, setLoading] = useState(false);

  const handleSubmit = (e) => {
    e.preventDefault();

    if (loading) return;

    setLoading(true);

    const payload = {
      name: e.target.name.value,
    };

    if (!!directoryId) {
      payload.parentDirectoryId = directoryId;
    }

    console.log({ payload });

    const token = loadToken();

    createDirectoryRequest(token, payload)
      .then((res) => {
        if (!res.success) {
          // do nothing
          return;
        }

        // reload on success
        window.location.reload();
      })
      .finally(() => {
        setLoading(false);
      });
  };

  return (
    <div
      style={{
        marginBottom: "30px",
        border: "1px solid black",
        padding: "15px",
      }}
    >
      <h2>Create Directory</h2>

      {loading && <p>Loading...</p>}

      <form onSubmit={handleSubmit}>
        <input type="text" name="name" id="" />

        <button type="submit">Create</button>
      </form>
    </div>
  );
};

export default CreateDirectory;
