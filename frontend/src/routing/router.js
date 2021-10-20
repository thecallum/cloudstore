import { BrowserRouter, Switch, Route } from "react-router-dom";

import ProtectedRoute from "./protectedRoute";

import Home from "../views/home";
import Login from "../views/login";
import Register from "../views/register";
import Account from "../views/account";
import Dashboard from "../views/dashboard/index";
import NotFoundPage from "../views/notFoundPage";

const Router = () => {
  return (
    <BrowserRouter>
      <Switch>
        <Route exact path="/" component={Home} />
        <Route exact path="/login" component={Login} />
        <Route exact path="/register" component={Register} />
        <ProtectedRoute exact path="/account" component={Account} />
        <ProtectedRoute path="/dashboard" component={Dashboard} />

        <Route component={NotFoundPage} />
      </Switch>
    </BrowserRouter>
  );
};

export default Router;
