import React, { useState } from "react";
import Validator from "Validator";

import loginRequest from "../requests/login";
import Layout from "./layout/layout";

import { saveToken } from "../services/authService";

const Login = ({ history }) => {
  const [loading, setLoading] = useState(false);
  const [fields, setFields] = useState({
    email: "",
    password: "",
  });
  const [errors, setErrors] = useState({});

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
          if (res.status === 401) {
            alert("unauthorized");
          } else {
            alert("bad request");
          }

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
        <div>
          <label class="form">Email</label>
          <input
            type="text"
            name="email"
            className={`form ${errors.hasOwnProperty("email") ? "error" : ""}`}
            value={fields.email}
            onChange={onInput}
          />
          {errors.hasOwnProperty("email") && (
            <span class="form">{errors.email[0]}</span>
          )}
        </div>

        <div>
          <label class="form">Password</label>
          <input
            type="password"
            name="password"
            className={`form ${
              errors.hasOwnProperty("password") ? "error" : ""
            }`}
            value={fields.password}
            onChange={onInput}
          />
          {errors.hasOwnProperty("password") && (
            <span class="form">{errors.password[0]}</span>
          )}
        </div>

        <button type="submit" class="form">
          Login
        </button>
      </form>

      {loading && <p>Loading...</p>}
    </Layout>
  );
};

export default Login;
