import React, { useState } from "react";
import Validator from "Validator";

import registerRequest from "../requests/register";
import Layout from "./layout/layout";

import TextInput from "../components/forms/TextInput";

const Register = ({ history }) => {
  const [loading, setLoading] = useState(false);
  const [fields, setFields] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    confirmPassword: ""
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
      firstName: "required|alpha|min:1|max:50",
      lastName: "required|alpha|min:1|max:50",
      email: "required|email|min:3|max:50",
      password: "required|string|min:8|max:100",
      confirmPassword: "same:password|string",
    };

    const messages = {
      "confirmPassword.same": "Password must match"
    }

    const v = Validator.make(fields, rules, messages);

    if (v.fails()) return v.getErrors();

    // valid
    return null;
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    console.log("handle submit")

    if (loading) return;

    const errors = validateRequest();

    // console.log({ errors })

    if (errors !== null) {
      setErrors(errors);
      return;
    }

    setErrors({});

    const requestBody = {
      ...fields,
    };

    setLoading(true);

    registerRequest(requestBody)
      .then((res) => {
        console.log({ res });

        if (res.success === false) {          
          if (res.status === 409) {
            setRequestError("Email already in use");
            return
          }

          setRequestError("Something went wrong");
          return;
        }

        history.push("/login/");
      })
      .finally(() => {
        setLoading(false);
      });
  };

  return (
    <Layout>
      <h1>Register</h1>
      <br />

      <form onSubmit={handleSubmit}>
      <TextInput
          name="firstName"
          label="First Name"
          value={fields.firstName}
          onChange={onInput}
          errors={errors["firstName"]}
        />

        <TextInput
          name="lastName"
          label="Last Name"
          value={fields.lastName}
          onChange={onInput}
          errors={errors["lastName"]}
        />

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

        <TextInput
          name="confirmPassword"
          label="Confirm Password"
          value={fields.confirmPassword}
          onChange={onInput}
          errors={errors["confirmPassword"]}
          type="password"
        />

        {requestError !== null && <span class="form">{requestError}</span>}

        {loading && <p>Loading...</p>}  

        <br />

        <button type="submit" className="form">
          Register
        </button>
      </form>

    </Layout>
  );

};

export default Register;
