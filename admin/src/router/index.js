import { createRouter, createWebHashHistory } from "vue-router";
import Layout from "../views/Layout.vue";
import Home from "../views/Home.vue";
import Login from "../views/Login.vue";

const routes = [
  {
    path: "/login",
    name: "Login",
    component: Login,
  },
  {
    path: "/test",
    name: "Test",
    component: () => import("../views/Test.vue"),
  },
  {
    path: "",
    component: Layout,
    children: [
      {
        path: "/",
        name: "Home",
        component: Home,
      },

      // {
      //   path: "/about",
      //   name: "About",
      //   // route level code-splitting
      //   // this generates a separate chunk (about.[hash].js) for this route
      //   // which is lazy-loaded when the route is visited.
      //   component: () =>
      //     import(/* webpackChunkName: "about" */ "../views/About.vue"),
      // },
      {
        path: "/apiscope",
        name: "ApiScope",
        component: () =>
          import(/* webpackChunkName: "apiscope" */ "../views/ApiScope.vue"),
      },
      {
        path: "/client",
        name: "Client",
        component: () =>
          import(/* webpackChunkName: "apiscope" */ "../views/Client.vue"),
      },
      {
        path: "/identity-resource",
        name: "IdentityResource",
        component: () =>
          import(
            /* webpackChunkName: "apiscope" */ "../views/IdentityResource.vue"
          ),
      },
      {
        path: "/api-resource",
        name: "ApiResource",
        component: () =>
          import(/* webpackChunkName: "apiscope" */ "../views/ApiResource.vue"),
      },
      {
        path: "/admin-user",
        name: "AdminUser",
        component: () =>
          import(/* webpackChunkName: "apiscope" */ "../views/AdminUser.vue"),
      },
    ],
  },
];

const router = createRouter({
  history: createWebHashHistory(),
  routes,
});

export default router;
