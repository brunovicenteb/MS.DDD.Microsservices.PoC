FROM node:alpine    
EXPOSE 3000
COPY Benefit.Consumer.Worker.Test/Mock/db.json /opt/db.json
RUN yarn global add json-server
ENTRYPOINT ["/app/entry.sh"]