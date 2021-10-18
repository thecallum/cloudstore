import React, { useState } from "react";
import loginRequest from "../requests/login";

import { saveToken } from "../services/authService";

export default ({ history }) => {
  const [loading, setLoading] = useState(false);

  const handleSubmit = (e) => {
    e.preventDefault();

    if (loading) return;

    const requestBody = {
      email: e.target.email.value,
      password: e.target.password.value,
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
    <>
      <h1>Login</h1>

      <form onSubmit={handleSubmit}>
        <input type="text" name="email" id="" />

        <input type="password" name="password" id="" />

        <button type="submit">Login</button>
      </form>

      {loading && <p>Loading...</p>}
    </>
  );
};
