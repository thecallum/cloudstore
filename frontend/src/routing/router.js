import {
  BrowserRouter,
  Switch,
  Route,
  Link,
  useLocation,
} from "react-router-dom";

import Home from "../views/home";
import Login from "../views/login";
import Register from "../views/register";
import Account from "../views/account";
import Dashboard from "../views/dashboard";
import NotFoundPage from "../views/notFoundPage";

const Router = () => {
  return (
    <BrowserRouter>
      <Switch>
        <Route exact path="/" component={Home} />
        <Route exact path="/login" component={Login} />
        <Route exact path="/register" component={Register} />
        <Route exact path="/account" component={Account} />
        <Route path="/dashboard" component={Dashboard} />

        <Route component={NotFoundPage} />
      </Switch>
    </BrowserRouter>
  );
};

export default Router;
