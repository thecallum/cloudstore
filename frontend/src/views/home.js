import React from "react";
import { Link } from "react-router-dom";
import Layout from "./layout/layout";

const Home = () => {
  return (
    <Layout>
      <h1>CloudStore is a service that allows you to store your files online.</h1>

      <br />  

      <p>Why not <Link to="/register/">Sign up</Link>?</p>
    </Layout>
  );
};

export default Home;
