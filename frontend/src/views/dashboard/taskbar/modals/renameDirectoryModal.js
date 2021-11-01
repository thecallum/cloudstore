import { loadToken } from "../../../../services/authService";
import renameDirectoryRequest from "../../../../requests/renameDirectory";
import { useState, useEffect } from "react";
import Validator from "Validator";

import TextInput from "../../../../components/forms/TextInput";

const RenameDirectoryModal = ({ directory, closeModal }) => {
  const originalName = directory.name;

  const [loading, setLoading] = useState(false);
  const [fields, setFields] = useState({ name: originalName });
  const [errors, setErrors] = useState({});
  const [requestError, setRequestError] = useState(null);

  // const [name, setName] = useState(originalName);

  const validateRequest = () => {
    const rules = {
      name: "required|string",
    };

    const v = Validator.make(fields, rules);

    if (v.fails()) return v.getErrors();

    // valid
    return null;
  };

  const onInput = (e) =>
    setFields({
      ...fields,
      [e.target.name]: e.target.value,
    });

  const handleSubmit = (e) => {
    e.preventDefault();
    if (loading) return;

    if (nameChanged === false) return;

    const errors = validateRequest();
    if (errors !== null) {
      setErrors(errors);
      return;
    }

    setErrors({});
    setRequestError(null);
    setLoading(true);

    const token = loadToken();

    renameDirectoryRequest(token, directory.directoryId, fields.name)
      .then((res) => {
        if (!res.success) {
          // do nothing

          setRequestError("Something went wrong");

          return;
        }

        window.location.reload();
      })
      .finally(() => {
        setLoading(false);
      });
  };

  const nameChanged = fields.name !== originalName;

  return (
    <div>
      <h2>Rename Directory</h2>

      <br />

      <form onSubmit={handleSubmit}>
        <TextInput
          name="name"
          label="Directory Name"
          value={fields.name}
          onChange={onInput}
          errors={errors["name"]}
        />

        {!!requestError && <span class="form">{requestError}</span>}

        {loading && <p>Loading...</p>}

        <div>
          <button
            type="submit"
            className="form"
            disabled={nameChanged === false || fields.name === ""}
          >
            Update
          </button>
        </div>
      </form>
    </div>
  );
};

export default RenameDirectoryModal;
