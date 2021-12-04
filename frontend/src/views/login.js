import React, { useState } from "react";
import Validator from "Validator";

import loginRequest from "../requests/login";
import Layout from "./layout/layout";

import TextInput from "../components/forms/TextInput";

import { saveToken } from "../services/authService";

const Login = ({ history }) => {
  const [loading, setLoading] = useState(false);
  const [fields, setFields] = useState({
    email: "",
    password: "",
  });
  const [errors, setErrors] = useState({});
  const [requestError, setRequestError] = useState(null);

  const onInput = (e) =>
    setFields({
      ...fields,
      [e.target.name]: e.target.value,
    });

  const validateRequest = () => {
    const rules = {
      email: "required|email",
      password: "required|string",
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

    const requestBody = {
      ...fields,
    };

    setLoading(true);

    loginRequest(requestBody)
      .then((res) => {
        console.log({ res });

        if (res.success === false) {
          setRequestError("Invalid password for the given email address");

          return;
        }

        saveToken(res.message);
        history.push("/dashboard/");
      })
      .finally(() => {
        setLoading(false);
      });
  };

  return (
    <Layout>
      <h1>Login</h1>
      <br />

      <form onSubmit={handleSubmit}>
        <TextInput
          name="email"
          label="Email"
          value={fields.email}
          onChange={onInput}
          errors={errors["email"]}
        />

        <TextInput
          name="password"
          label="Password"
          value={fields.password}
          onChange={onInput}
          errors={errors["password"]}
          type="password"
        />

        {requestError !== null && <span class="form">{requestError}</span>}

        {loading && <p>Loading...</p>}

        <br />

        <button type="submit" class="form">
          Login
        </button>
      </form>
    </Layout>
  );
};

export default Login;
