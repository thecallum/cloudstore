import Validator from "Validator";

import { loadToken } from "../../../../services/authService";
import createDirectoryRequest from "../../../../requests/createDirectory";
import { useState } from "react";

import TextInput from "../../../../components/forms/TextInput";

const CreateDirectoryModal = ({ closeModal, directoryId }) => {
  const [loading, setLoading] = useState(false);
  const [fields, setFields] = useState({ name: "" });
  const [errors, setErrors] = useState({});
  const [requestError, setRequestError] = useState(null);

  const onInput = (e) =>
    setFields({
      ...fields,
      [e.target.name]: e.target.value,
    });

  const validateRequest = () => {
    const rules = {
      name: "required|string",
    };

    const v = Validator.make(fields, rules);

    if (v.fails()) return v.getErrors();

    // valid
    return null;
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    if (loading) return;

    const errors = validateRequest();

    if (errors !== null) {
      setErrors(errors);
      return;
    }

    setErrors({});

    const payload = {
      name: e.target.name.value,
    };

    setLoading(true);

    if (!!directoryId) {
      payload.parentDirectoryId = directoryId;
    }

    const token = loadToken();

    createDirectoryRequest(token, payload)
      .then((res) => {
        if (!res.success) {
          // do nothing

          setRequestError("Something went wrong");
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

      <br />

      <form onSubmit={handleSubmit}>
        <TextInput
          name="name"
          label="Directory Name"
          value={fields.name}
          onChange={onInput}
          errors={errors["name"]}
        />

        {requestError !== null && <span class="form">{requestError}</span>}

        {loading && <p>Loading...</p>}

        <button type="submit" class="form">
          Create
        </button>
      </form>
    </div>
  );
};

export default CreateDirectoryModal;
