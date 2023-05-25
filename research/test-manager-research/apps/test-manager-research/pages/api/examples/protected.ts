// This is an example of to protect an API route
import { getServerSession } from "next-auth/next"

import type { NextApiRequest, NextApiResponse } from "next"
import { authOptions } from "apps/test-manager-research/app/api/auth/[...nextauth]/route";
//import axios from "axios"

export default async function handler(
  req: NextApiRequest,
  res: NextApiResponse
) {
  const session = await getServerSession(req, res, authOptions)
  process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = "0";
  // axios.get('https://dev.test-manager-api.testmaster.io/Management/TestDefinition?pageNumber=1&pageSize=12', {
  //   headers: {
  //     'Authorization': `Bearer ${session?.accessToken ?? ''}`
  //   }
  // }).then(data => res.send(data.data));
  fetch('https://dev.test-manager-api.testmaster.io/Management/TestDefinition?pageNumber=1&pageSize=12', {
    method: req.method,
    headers: {
      'Authorization': `Bearer ${session?.accessToken ?? ''}`
    }
  }).then(async rs => res.send(await rs.json()));
}
